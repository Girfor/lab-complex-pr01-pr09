using System.Globalization;
using AIS.Core;

namespace AIS.Infrastructure;

public sealed record SupplyItem(
    string SupplyId, string MaterialCode, string Title, string Lot,
    string Qty, string Uom, string ExpiresOn, string Vendor);

public static class SupplyMapper
{
    public static ReceiveReagentRequest Convert(SupplyItem source)
    {
        if (!decimal.TryParse(source.Qty, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out var quantity) || quantity <= 0)
            throw new ArgumentException("qty должен быть положительным числом с точкой");
        if (!DateOnly.TryParseExact(source.ExpiresOn, "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var expires))
            throw new ArgumentException("expiresOn должен иметь формат yyyy-MM-dd");

        var unit = source.Uom.ToLowerInvariant();
        switch (unit)
        {
            case "kg": quantity *= 1000m; unit = "g"; break;
            case "g": case "ml": break;
            default: throw new ArgumentException("Неизвестная единица измерения");
        }
        return new ReceiveReagentRequest(source.Title.Trim(),
            source.MaterialCode.Trim(), source.Lot.Trim(), quantity,
            unit, expires, source.Vendor.Trim(), source.SupplyId.Trim());
    }
}
