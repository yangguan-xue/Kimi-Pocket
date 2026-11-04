using System.Text;
using System.Text.Json;
namespace KimiPocket.Core.Auth;

public sealed record TokenClaims(string Subject, string Session, DateTimeOffset ExpiresAt);
public static class JwtPrecheck
{
    public static TokenClaims Parse(string token, DateTimeOffset now)
    {
        try
        {
            var p = token.Trim().Split('.'); if (p.Length != 3 || p.Any(s => string.IsNullOrWhiteSpace(s) || s.Any(c => !char.IsAsciiLetterOrDigit(c) && c != '-' && c != '_')) || token.Length > 16384) throw new FormatException();
            var b = p[1].Replace('-', '+').Replace('_', '/'); b = b.PadRight((b.Length + 3) / 4 * 4, '='); using var d = JsonDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(b)));
            var r = d.RootElement; var sub = r.GetProperty("sub").GetString(); var session = r.GetProperty("ssid").GetString();
            if (string.IsNullOrWhiteSpace(sub) || string.IsNullOrWhiteSpace(session) || sub.Any(char.IsControl) || session.Any(char.IsControl)) throw new FormatException();
            var exp = DateTimeOffset.FromUnixTimeSeconds(r.GetProperty("exp").GetInt64()); if (now >= exp) throw new TokenExpiredException(); return new(sub, session, exp);
        }
        catch (TokenExpiredException) { throw; }
        catch (Exception e) when (e is JsonException or FormatException or InvalidOperationException or KeyNotFoundException or ArgumentOutOfRangeException) { throw new FormatException("Token 格式不正确，请粘贴完整的浏览器 access_token"); }
    }
}
public sealed class TokenExpiredException : Exception { public TokenExpiredException() : base("登录凭证已过期，请更新 Token") { } }
