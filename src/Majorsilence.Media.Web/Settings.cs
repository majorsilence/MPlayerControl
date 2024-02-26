namespace Majorsilence.Media.Web;

public class Settings
{
    public string UploadFolder { get; init; }
    public JwtSettings Jwt { get; init; }
    public string[] PermittedCORSOrigins { get; init; }
}