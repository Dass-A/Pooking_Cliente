using System.Text;
using Microservicio.Pooking.Cliente.Api.Models.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Microservicio.Pooking.Cliente.Api.Extensions;

/// <summary>
/// Configura autenticación JWT Bearer.
/// Lee JwtSettings desde appsettings.json.
/// Si JwtSettings.Enabled = false, no se exige token (útil en desarrollo).
/// </summary>
public static class AuthenticationExtensions
{
    public static IServiceCollection AddCustomAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>()
            ?? throw new InvalidOperationException(
                "No se encontró la configuración JwtSettings en appsettings.json.");

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        if (!jwtSettings.Enabled)
        {
            // Aún registramos el esquema pero sin validar firmas reales (modo dev).
            // Los [Authorize] seguirán funcionando si se mandan tokens, pero no son obligatorios
            // a nivel global. En este microservicio cada controlador decide con [Authorize].
            return services;
        }

        var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // true en producción
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }
}
