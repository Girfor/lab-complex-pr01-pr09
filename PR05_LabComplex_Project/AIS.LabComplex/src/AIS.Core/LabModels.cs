namespace AIS.Core;

public enum UserRole { Administrator, LaboratoryAssistant, Researcher }

public sealed record Reagent(
    Guid Id, string Name, string ExternalCode, string Lot,
    decimal Quantity, string Unit, DateOnly ExpirationDate,
    string SupplierCode, string DeliveryId);

public sealed record Equipment(string Id, string Name, bool IsAvailable);

public sealed record Booking(
    Guid Id, string EquipmentId, Guid UserId,
    DateTimeOffset StartsAt, DateTimeOffset EndsAt);

public sealed record LabEvent(
    DateTimeOffset OccurredAt, string Action,
    string ObjectId, Guid UserId, string Details);

public sealed record ReceiveReagentRequest(
    string Name, string ExternalCode, string Lot,
    decimal Quantity, string Unit, DateOnly ExpirationDate,
    string SupplierCode, string DeliveryId);

public sealed record CreateEquipmentRequest(string Id, string Name);
public sealed record CreateBookingRequest(
    string EquipmentId, DateTimeOffset StartsAt, DateTimeOffset EndsAt);

public sealed record AccessRequest(
    Guid RequestId, Guid UserId, string EquipmentId,
    string Action, DateTimeOffset RequestedAt);

public sealed record AccessResponse(
    Guid RequestId, bool Allowed, Guid? SessionId,
    DateTimeOffset? ValidUntil);
