using AIS.Core;
using AIS.Infrastructure;

var service = new LabService();
var admin = Guid.NewGuid();
var researcher = Guid.NewGuid();
int passed = 0;

void Check(bool condition, string name)
{
    if (!condition) throw new Exception("FAILED: " + name);
    passed++;
    Console.WriteLine("PASS: " + name);
}

var mapped = SupplyMapper.Convert(new SupplyItem(
    "D1", "CHEM-1", "Соль", "LOT-A", "2.50", "kg", "2028-01-01", "V1"));
Check(mapped.Quantity == 2500m && mapped.Unit == "g", "mapping kg->g");

var r1 = service.Receive(UserRole.LaboratoryAssistant, admin, mapped);
var r2 = service.Receive(UserRole.LaboratoryAssistant, admin, mapped);
Check(r1.Id == r2.Id && service.SearchReagents(null).Count == 1, "idempotent delivery");

var updated = service.WriteOff(UserRole.LaboratoryAssistant, admin, r1.Id, 500m);
Check(updated.Quantity == 2000m, "write-off");

service.RegisterEquipment(UserRole.Administrator, admin,
    new CreateEquipmentRequest("EQ-0042", "Микроскоп"));
var start = DateTimeOffset.UtcNow.AddMinutes(-10);
service.Book(UserRole.Researcher, researcher,
    new CreateBookingRequest("EQ-0042", start, start.AddHours(1)));
try
{
    service.Book(UserRole.Researcher, researcher,
        new CreateBookingRequest("EQ-0042", start, start.AddMinutes(30)));
    throw new Exception("Double booking was accepted");
}
catch (InvalidOperationException) { passed++; Console.WriteLine("PASS: overlapping booking rejected"); }

var access = service.CheckAccess(UserRole.Researcher, researcher,
    new AccessRequest(Guid.NewGuid(), researcher, "EQ-0042", "START_SESSION",
        DateTimeOffset.UtcNow), DateTimeOffset.UtcNow);
Check(access.Allowed && access.SessionId.HasValue, "authorized device session");

service.DisableEquipment(UserRole.Administrator, admin, "EQ-0042");
Check(!service.GetEquipment().Single().IsAvailable, "device disabled");

Console.WriteLine($"Tests passed: {passed}");
