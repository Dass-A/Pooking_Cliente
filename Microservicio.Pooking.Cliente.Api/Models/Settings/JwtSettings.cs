namespace Microservicio.Pooking.Cliente.Api.Models.Settings;

public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";

    /// <summary>Si es false, NO se exige token en los endpoints (útil en desarrollo).</summary>
    public bool Enabled { get; set; }

    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}
