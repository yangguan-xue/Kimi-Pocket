using System.Globalization;
using System.Text.Json;
namespace KimiPocket.Core.Usage;

public static class UsageNormalizer
{
    private static JsonElement Get(JsonElement obj, string key) => obj.ValueKind == JsonValueKind.Object && obj.TryGetProperty(key, out var v) ? v : default;
    private static string? Text(JsonElement e) => e.ValueKind == JsonValueKind.String ? e.GetString() : null;
    private static double? Number(JsonElement e) { var text = e.ValueKind == JsonValueKind.Number ? e.GetRawText() : Text(e); return double.TryParse(text, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var n) && double.IsFinite(n) ? n : null; }
    private static DateTimeOffset? Date(JsonElement e) => DateTimeOffset.TryParse(Text(e), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var d) ? d : null;
    private static Quota? ParseQuota(JsonElement e) { var l = Number(Get(e, "limit")); var r = Number(Get(e, "remaining")); return l is > 0 && r is >= 0 && r <= l ? new(l.Value, r.Value, Date(Get(e, "resetTime"))) : null; }
    public static Plan? ParsePlan(string json)
    {
        using var d = JsonDocument.Parse(json); var s = Get(d.RootElement, "subscription"); var m = Get(d.RootElement, "membership");
        if (s.ValueKind != JsonValueKind.Object && m.ValueKind != JsonValueKind.Object) return null;
        var title = Text(Get(Get(s, "goods"), "title")) ?? Text(Get(m, "title")) ?? Text(Get(m, "level"));
        var end = Date(Get(s, "currentEndTime")) ?? Date(Get(m, "endTime"));
        var active = Get(s, "active"); return new(title, end, active.ValueKind is JsonValueKind.True or JsonValueKind.False ? active.GetBoolean() : null);
    }
    public static UsageData ParseUsage(string json)
    {
        using var d = JsonDocument.Parse(json); var root = d.RootElement; if (root.ValueKind != JsonValueKind.Object) throw new JsonException("Expected usage object"); var total = ParseQuota(Get(root, "totalQuota")); Quota? weekly = null, rolling = null;
        var usages = Get(root, "usages"); if (usages.ValueKind == JsonValueKind.Array)
            foreach (var u in usages.EnumerateArray())
            {
                if (Text(Get(u, "scope")) != "FEATURE_CODING") continue;
                weekly = ParseQuota(Get(u, "detail")); var limits = Get(u, "limits");
                if (limits.ValueKind == JsonValueKind.Array) foreach (var l in limits.EnumerateArray())
                {
                    var w = Get(l, "window"); var unit = Text(Get(w, "timeUnit")); var duration = Number(Get(w, "duration"));
                    if ((unit == "TIME_UNIT_MINUTE" && duration == 300) || (unit == "TIME_UNIT_HOUR" && duration == 5)) { rolling = ParseQuota(Get(l, "detail")); break; }
                }
                break;
            }
        return new(total, weekly, rolling);
    }
}
