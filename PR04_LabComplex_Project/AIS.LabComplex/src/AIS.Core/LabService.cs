using System.Collections.ObjectModel;

namespace AIS.Core;

/// <summary>Учебная бизнес-логика; данные живут в памяти одного процесса.</summary>
public sealed class LabService
{
    private readonly object _gate = new();
    private readonly Dictionary<Guid, Reagent> _reagents = new();
    private readonly Dictionary<string, Equipment> _equipment =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly List<Booking> _bookings = new();
    private readonly List<LabEvent> _events = new();
    private readonly Dictionary<string, Guid> _deliveries =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<Guid, AccessResponse> _accessRequests = new();

    public IReadOnlyList<Reagent> SearchReagents(string? query)
    {
        lock (_gate)
            return _reagents.Values
                .Where(r => string.IsNullOrWhiteSpace(query) ||
                    r.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .OrderBy(r => r.Name)
                .ToArray();
    }

    public IReadOnlyList<Equipment> GetEquipment()
    {
        lock (_gate) return _equipment.Values.OrderBy(x => x.Id).ToArray();
    }

    public IReadOnlyList<Booking> GetBookings(string? equipmentId = null)
    {
        lock (_gate)
            return _bookings.Where(x => equipmentId is null ||
                x.EquipmentId.Equals(equipmentId, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.StartsAt).ToArray();
    }

    public IReadOnlyList<LabEvent> GetEvents()
    {
        lock (_gate) return _events.ToArray();
    }

    public Reagent Receive(UserRole role, Guid actor, ReceiveReagentRequest data)
    {
        Require(role, UserRole.Administrator, UserRole.LaboratoryAssistant);
        if (string.IsNullOrWhiteSpace(data.Name) || data.Name.Length > 200)
            throw new ArgumentException("Название реактива обязательно, максимум 200 символов");
        if (data.Quantity <= 0 || data.Quantity > 1_000_000_000)
            throw new ArgumentException("Недопустимое количество реактива");
        if (data.Unit is not ("g" or "ml"))
            throw new ArgumentException("Допускаются единицы g и ml");
        if (string.IsNullOrWhiteSpace(data.Lot) ||
            string.IsNullOrWhiteSpace(data.ExternalCode) ||
            string.IsNullOrWhiteSpace(data.DeliveryId))
            throw new ArgumentException("Нужны код вещества, партия и поставка");

        var key = data.DeliveryId.Trim() + "|" + data.Lot.Trim();
        lock (_gate)
        {
            if (_deliveries.TryGetValue(key, out var existingId))
            {
                var old = _reagents[existingId];
                if (old.ExternalCode != data.ExternalCode || old.Quantity != data.Quantity)
                    throw new InvalidOperationException("Коллизия повторной поставки");
                return old; // идемпотентность: остаток не увеличивается дважды
            }
            var r = new Reagent(Guid.NewGuid(), data.Name.Trim(),
                data.ExternalCode.Trim(), data.Lot.Trim(), data.Quantity,
                data.Unit, data.ExpirationDate, data.SupplierCode.Trim(),
                data.DeliveryId.Trim());
            _reagents.Add(r.Id, r);
            _deliveries.Add(key, r.Id);
            AddEvent("reagent.received", r.Id.ToString(), actor, data.Quantity.ToString());
            return r;
        }
    }

    public Reagent WriteOff(UserRole role, Guid actor, Guid reagentId, decimal quantity)
    {
        Require(role, UserRole.Administrator, UserRole.LaboratoryAssistant);
        lock (_gate)
        {
            if (!_reagents.TryGetValue(reagentId, out var current))
                throw new KeyNotFoundException("Реактив не найден");
            if (quantity <= 0 || quantity > current.Quantity)
                throw new InvalidOperationException("Недостаточно реактива для списания");
            var updated = current with { Quantity = current.Quantity - quantity };
            _reagents[reagentId] = updated;
            AddEvent("reagent.written_off", reagentId.ToString(), actor, quantity.ToString());
            return updated;
        }
    }

    public Equipment RegisterEquipment(UserRole role, Guid actor, CreateEquipmentRequest data)
    {
        Require(role, UserRole.Administrator);
        if (string.IsNullOrWhiteSpace(data.Id) ||
            !System.Text.RegularExpressions.Regex.IsMatch(data.Id, "^EQ-[0-9]{4,8}$") ||
            string.IsNullOrWhiteSpace(data.Name))
            throw new ArgumentException("Ожидаются код EQ-0001 и название");
        lock (_gate)
        {
            if (_equipment.ContainsKey(data.Id))
                throw new InvalidOperationException("Оборудование уже зарегистрировано");
            var equipment = new Equipment(data.Id, data.Name.Trim(), true);
            _equipment.Add(equipment.Id, equipment);
            AddEvent("equipment.registered", equipment.Id, actor, equipment.Name);
            return equipment;
        }
    }

    public Equipment DisableEquipment(UserRole role, Guid actor, string id)
    {
        Require(role, UserRole.Administrator);
        lock (_gate)
        {
            if (!_equipment.TryGetValue(id, out var current))
                throw new KeyNotFoundException("Оборудование не найдено");
            var updated = current with { IsAvailable = false };
            _equipment[id] = updated;
            AddEvent("equipment.disabled", id, actor, "На обслуживании");
            return updated;
        }
    }

    public Booking Book(UserRole role, Guid actor, CreateBookingRequest data)
    {
        Require(role, UserRole.Administrator, UserRole.LaboratoryAssistant, UserRole.Researcher);
        if (data.EndsAt <= data.StartsAt)
            throw new ArgumentException("Конец интервала должен быть позже начала");
        lock (_gate)
        {
            if (!_equipment.TryGetValue(data.EquipmentId, out var equipment))
                throw new KeyNotFoundException("Оборудование не найдено");
            if (!equipment.IsAvailable)
                throw new InvalidOperationException("Прибор на обслуживании");
            if (_bookings.Any(b => b.EquipmentId.Equals(data.EquipmentId,
                    StringComparison.OrdinalIgnoreCase) &&
                    b.StartsAt < data.EndsAt && data.StartsAt < b.EndsAt))
                throw new InvalidOperationException("Пересечение с существующей бронью");
            var booking = new Booking(Guid.NewGuid(), equipment.Id, actor,
                data.StartsAt, data.EndsAt);
            _bookings.Add(booking);
            AddEvent("booking.created", booking.Id.ToString(), actor, equipment.Id);
            return booking;
        }
    }

    public AccessResponse CheckAccess(
        UserRole role, Guid actor, AccessRequest request, DateTimeOffset now)
    {
        Require(role, UserRole.Administrator, UserRole.LaboratoryAssistant, UserRole.Researcher);
        if (request.RequestId == Guid.Empty || request.UserId != actor ||
            !System.Text.RegularExpressions.Regex.IsMatch(request.EquipmentId, "^EQ-[0-9]{4,8}$") ||
            request.Action is not ("START_SESSION" or "END_SESSION") ||
            Math.Abs((now - request.RequestedAt).TotalMinutes) > 5)
            throw new ArgumentException("Некорректный запрос доступа");

        lock (_gate)
        {
            if (_accessRequests.TryGetValue(request.RequestId, out var prior))
                return prior;
            if (!_equipment.TryGetValue(request.EquipmentId, out var device))
                throw new KeyNotFoundException("Оборудование не найдено");
            if (request.Action == "END_SESSION")
                throw new InvalidOperationException("Остановка сессии еще не реализована");
            var booking = _bookings.FirstOrDefault(b => b.UserId == actor &&
                b.EquipmentId.Equals(request.EquipmentId, StringComparison.OrdinalIgnoreCase) &&
                b.StartsAt <= now && now < b.EndsAt);
            if (!device.IsAvailable || booking is null)
                throw new InvalidOperationException("Нет активной брони или прибор занят");
            var response = new AccessResponse(request.RequestId, true,
                Guid.NewGuid(), booking.EndsAt);
            _accessRequests.Add(request.RequestId, response);
            AddEvent("access.granted", device.Id, actor, response.SessionId.ToString()!);
            return response;
        }
    }

    private void AddEvent(string action, string objectId, Guid actor, string details) =>
        _events.Add(new LabEvent(DateTimeOffset.UtcNow, action, objectId, actor, details));

    private static void Require(UserRole actual, params UserRole[] allowed)
    {
        if (!allowed.Contains(actual))
            throw new UnauthorizedAccessException("Недостаточно прав");
    }
}
