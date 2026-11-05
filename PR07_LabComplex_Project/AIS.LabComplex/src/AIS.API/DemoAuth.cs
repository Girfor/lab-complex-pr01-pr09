using System.Net;
using System.Security.Cryptography;
using System.Text;
using AIS.Core;

namespace AIS.API;

/// <summary>
/// ТОЛЬКО для локальной учебной демонстрации. В реальной системе заменить OIDC.
/// Ни HTTP, ни встроенные ключи не подходят для сетевой эксплуатации.
/// </summary>
public sealed record DemoIdentity(Guid UserId, UserRole Role);

public static class DemoAuth
{
    public static DemoIdentity? Resolve(HttpContext ctx, IConfiguration config,
        IWebHostEnvironment environment)
    {
        if (!environment.IsDevelopment() ||
            ctx.Connection.RemoteIpAddress is not { } ip || !IPAddress.IsLoopback(ip))
            return null;

        var provided = ctx.Request.Headers["X-Demo-Key"].ToString();
        if (string.IsNullOrEmpty(provided)) return null;

        var definitions = new[]
        {
            ("Admin", UserRole.Administrator, "6db4ba5a-7332-4fb6-950d-bc685901e320"),
            ("LabAssistant", UserRole.LaboratoryAssistant, "210bcf5b-3cb5-49bb-9347-ccde6a99ff55"),
            ("Researcher", UserRole.Researcher, "91d8b349-1747-4b58-a0fd-2297669fbc14")
        };
        foreach (var (configName, role, userId) in definitions)
        {
            var expected = config[$"DemoKeys:{configName}"];
            if (expected is null) continue;
            var a = Encoding.UTF8.GetBytes(provided);
            var b = Encoding.UTF8.GetBytes(expected);
            if (a.Length == b.Length && CryptographicOperations.FixedTimeEquals(a, b))
                return new DemoIdentity(Guid.Parse(userId), role);
        }
        return null;
    }
}
