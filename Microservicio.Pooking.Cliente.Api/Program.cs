using Microservicio.Pooking.Cliente.Api.Extensions;
using Microservicio.Pooking.Cliente.Api.Middleware;
using Microservicio.Pooking.Cliente.Api.Models.Settings;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Permitir HTTP/2 sin TLS en desarrollo local (necesario para gRPC sin cert configurado).
// En producción usar siempre HTTPS.
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------------------------------
// Servicios base
// -------------------------------------------------------------------------
Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

builder.Services.AddControllers();

// -------------------------------------------------------------------------
// Configuraciones transversales
// -------------------------------------------------------------------------
builder.Services.AddCustomApiVersioning();
builder.Services.AddPookingCors(builder.Configuration);
builder.Services.AddCustomAuthentication(builder.Configuration);
builder.Services.AddCustomSwagger();
builder.Services.AddAuthorization();

// -------------------------------------------------------------------------
// Módulo del dominio Cliente (registra DbContext, UoW, DataServices, Services)
// -------------------------------------------------------------------------
builder.Services.AddClienteModule(builder.Configuration);

// -------------------------------------------------------------------------
// Cliente gRPC hacia Booking.Middleware
// (único microservicio gRPC que este servicio consume — regla del plan v2.0)
// -------------------------------------------------------------------------
builder.Services.AddMiddlewareGrpcClient(builder.Configuration);

// -------------------------------------------------------------------------
// Pipeline HTTP
// -------------------------------------------------------------------------
var app = builder.Build();

// Middleware global de errores — debe ser el PRIMERO
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Microservicio Pooking Cliente API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors(CorsExtensions.PolicyName);

// Autenticación solo si está habilitada (solo afecta REST — gRPC no se expone)
var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>();

if (jwtSettings?.Enabled == true)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.MapControllers();

app.Run();
