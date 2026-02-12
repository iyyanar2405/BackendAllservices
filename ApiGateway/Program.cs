using Serilog;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using ApiGateway.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add authentication (JWT Bearer)
var jwtSection = builder.Configuration.GetSection("Jwt");
var secret = jwtSection["SecretKey"] ?? "";
var issuer = jwtSection["Issuer"] ?? "";
var audience = jwtSection["Audience"] ?? "";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret))
        };
    });

// Add Ocelot
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

// Exception handling should be first to catch downstream errors
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Correlation id for tracing requests across services
app.UseMiddleware<CorrelationIdMiddleware>();

// Request/response logging
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

app.Run();
