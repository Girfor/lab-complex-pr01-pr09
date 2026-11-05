using AIS.Core;
using AIS.Infrastructure;

Console.OutputEncoding = System.Text.Encoding.UTF8;
var service = new LabService();
var admin = Guid.NewGuid();
var researcher = Guid.NewGuid();

service.RegisterEquipment(UserRole.Administrator, admin,
    new CreateEquipmentRequest("EQ-0042", "Лабораторный микроскоп"));

var incoming = SupplyMapper.Convert(new SupplyItem(
    "DEL-2026-0178", "CHEM-0091", "Натрий хлористый", "L-247A",
    "2.50", "kg", "2027-10-31", "SUP-012"));
var reagent = service.Receive(UserRole.LaboratoryAssistant, admin, incoming);
Console.WriteLine($"Прием: {reagent.Name}, {reagent.Quantity} {reagent.Unit}");

var start = DateTimeOffset.UtcNow.AddHours(1);
var booking = service.Book(UserRole.Researcher, researcher,
    new CreateBookingRequest("EQ-0042", start, start.AddHours(2)));
Console.WriteLine($"Бронь: {booking.Id}, {booking.StartsAt:u}");

try
{
    service.Book(UserRole.Researcher, researcher,
        new CreateBookingRequest("EQ-0042", start, start.AddHours(1)));
}
catch (InvalidOperationException e)
{
    Console.WriteLine("Ожидаемый отказ: " + e.Message);
}

service.DisableEquipment(UserRole.Administrator, admin, "EQ-0042");
Console.WriteLine("Прибор переведен на обслуживание");
