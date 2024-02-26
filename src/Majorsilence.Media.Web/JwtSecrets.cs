namespace Majorsilence.Media.Web;

public class JwtSettings
{
    public string Audience { get; init; }
    public string Issuer { get; init; }
    public string Secret { get; init; }
}