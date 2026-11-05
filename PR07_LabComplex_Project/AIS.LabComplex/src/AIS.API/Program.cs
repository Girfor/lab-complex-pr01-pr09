using AIS.API;
using AIS.Core;
using AIS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<LabService>();
var app = builder.Build();

// Безопасная по умолчанию граница: тестовые ключи действуют только на loopback
// и только в Development. В Production интеграция OIDC обязательна.
app.Use(async (ctx, next) =>
{
    if (!ctx.Request.Path.StartsWithSegments("/api/v1"))
    {
        await next();
        return;
    }
    var identity = DemoAuth.Resolve(ctx, app.Configuration, app.Environment);
    if (identity is null)
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await ctx.Response.WriteAsJsonAsync(new { code = "UNAUTHENTICATED" });
        return;
    }
    ctx.Items["identity"] = identity;
    await next();
});

app.MapGet("/health", () => Results.Ok(new { status = "healthy", system = "AIS.LabComplex" }));
var api = app.MapGroup("/api/v1");

api.MapGet("/reagents", (LabService service, string? q, int? page, int? pageSize) =>
{
    var p = Math.Max(1, page ?? 1);
    var size = Math.Clamp(pageSize ?? 20, 1, 100);
    var all = service.SearchReagents(q);
    return Results.Ok(new
    {
        items = all.Skip((p - 1) * size).Take(size),
        total = all.Count,
        page = p,
        pageSize = size
    });
});

api.MapPost("/reagents", (LabService service, HttpContext ctx, ReceiveReagentRequest req) =>
    Safe(() =>
    {
        var who = Current(ctx);
        var created = service.Receive(who.Role, who.UserId, req);
        return Results.Created($"/api/v1/reagents/{created.Id}", created);
    }));

api.MapPost("/reagents/{id:guid}/write-off", (LabService service,
    HttpContext ctx, Guid id, WriteOffRequest req) =>
    Safe(() => Results.Ok(service.WriteOff(Current(ctx).Role,
        Current(ctx).UserId, id, req.Quantity))));

api.MapGet("/equipment", (LabService service) => Results.Ok(service.GetEquipment()));

api.MapPost("/equipment", (LabService service, HttpContext ctx, CreateEquipmentRequest req) =>
    Safe(() =>
    {
        var who = Current(ctx);
        var result = service.RegisterEquipment(who.Role, who.UserId, req);
        return Results.Created($"/api/v1/equipment/{result.Id}", result);
    }));

api.MapPost("/equipment/{id}/disable", (LabService service, HttpContext ctx, string id) =>
    Safe(() => Results.Ok(service.DisableEquipment(Current(ctx).Role,
        Current(ctx).UserId, id))));

api.MapGet("/bookings", (LabService service, string? equipmentId) =>
    Results.Ok(service.GetBookings(equipmentId)));

api.MapPost("/bookings", (LabService service, HttpContext ctx, CreateBookingRequest req) =>
    Safe(() =>
    {
        var who = Current(ctx);
        var result = service.Book(who.Role, who.UserId, req);
        return Results.Created($"/api/v1/bookings/{result.Id}", result);
    }));

api.MapPost("/equipment/access", (LabService service, HttpContext ctx, AccessRequest req) =>
    Safe(() =>
    {
        var who = Current(ctx);
        return Results.Ok(service.CheckAccess(who.Role, who.UserId,
            req, DateTimeOffset.UtcNow));
    }));

api.MapPost("/integrations/supply", (LabService service, HttpContext ctx, SupplyItem req) =>
    Safe(() =>
    {
        var who = Current(ctx);
        var data = SupplyMapper.Convert(req);
        return Results.Ok(service.Receive(who.Role, who.UserId, data));
    }));

api.MapGet("/events", (LabService service, HttpContext ctx) =>
    Current(ctx).Role == UserRole.Administrator
        ? (IResult)Results.Ok(service.GetEvents())
        : Results.StatusCode(StatusCodes.Status403Forbidden));

app.Run();

static DemoIdentity Current(HttpContext ctx) =>
    (DemoIdentity)ctx.Items["identity"]!;

static IResult Safe(Func<IResult> work)
{
    try { return work(); }
    catch (UnauthorizedAccessException ex)
    { return Results.Problem(title: ex.Message, statusCode: 403); }
    catch (KeyNotFoundException ex)
    { return Results.Problem(title: ex.Message, statusCode: 404); }
    catch (ArgumentException ex)
    { return Results.Problem(title: ex.Message, statusCode: 422); }
    catch (InvalidOperationException ex)
    { return Results.Problem(title: ex.Message, statusCode: 409); }
}

public sealed record WriteOffRequest(decimal Quantity);
