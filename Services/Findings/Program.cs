// Program.cs
using FindingsAPI.Gateway;
using FindingsAPI.Gateway.Data;
using FindingsAPI.Gateway.GraphQL;
using FindingsAPI.Gateway.GraphQL.Middleware;
using FindingsAPI.Gateway.GraphQL.Queries;
using FindingsAPI.Gateway.GraphQL.Mutations;
// using FindingsAPI.Gateway.GraphQL.Subscriptions;
using FindingsAPI.Gateway.GraphQL.Types;
using FindingsAPI.Gateway.GraphQL.DataLoaders;
using FindingsAPI.Gateway.Middleware;
using FindingsAPI.Gateway.Services;
using FindingsAPI.Gateway.Repositories;
using HealthChecks.UI.Client;
using HotChocolate;
using HotChocolate.AspNetCore;
using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {CorrelationId} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// GraphQL Server
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    // .AddSubscriptionType<Subscription>()
    .AddType<FindingType>()
    .AddType<CompanyType>()
    .AddType<SiteType>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    // .AddAuthorization()
    .AddHttpRequestInterceptor<GraphQLHttpRequestInterceptor>()
    // .AddSocketSessionInterceptor<GraphQLSocketSessionInterceptor>()
    .AddDiagnosticEventListener<GraphQLExecutionLogger>()
    .AddErrorFilter<GraphQLErrorFilter>()
    .AddDataLoader<CompanyDataLoader>()
    // .AddDataLoader<SiteDataLoader>()
    // .PublishSchema(c => c
    //     .SetName("findings")
    //     .PublishToRedis("FindingsSchema", sp => sp.GetRequiredService<IConnectionMultiplexer>()))
    // .AddApolloTracing()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = builder.Environment.IsDevelopment());

// HTTP Clients with Polly policies - REMOVED: No longer needed as services now use database directly
// builder.Services.AddHttpClient("FindingsService", client =>
// {
//     client.BaseAddress = new Uri(builder.Configuration["Services:FindingsService"]);
//     client.DefaultRequestHeaders.Add("X-API-Version", "1.0");
// })
// .AddHttpMessageHandler<CorrelationIdHandler>();

// builder.Services.AddHttpClient("CompanyService", client =>
// {
//     client.BaseAddress = new Uri(builder.Configuration["Services:CompanyService"]);
//     client.DefaultRequestHeaders.Add("X-API-Version", "1.0");
// })
// .AddHttpMessageHandler<CorrelationIdHandler>();

// builder.Services.AddHttpClient("SiteService", client =>
// {
//     client.BaseAddress = new Uri(builder.Configuration["Services:SiteService"]);
//     client.DefaultRequestHeaders.Add("X-API-Version", "1.0");
// })
// .AddHttpMessageHandler<CorrelationIdHandler>();

// Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };

    // For GraphQL WebSocket authentication
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            
            if (!string.IsNullOrEmpty(accessToken) && 
                path.StartsWithSegments("/graphql"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => 
        policy.RequireClaim("role", "admin"));
    
    options.AddPolicy("Auditor", policy => 
        policy.RequireClaim("role", "auditor", "admin"));
    
    options.AddPolicy("User", policy => 
        policy.RequireClaim("role", "user", "auditor", "admin"));
    
    // GraphQL specific policies
    options.AddPolicy("CanViewFindings", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => 
                c.Type == "permissions" && 
                c.Value.Contains("findings:read"))));
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("GatewayCors", policyBuilder =>
    {
        policyBuilder.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>())
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials()
               .WithExposedHeaders("X-Correlation-Id", "X-Request-Id");
    });
});

// Health Checks
// builder.Services.AddHealthChecks()
//     // .AddUrlGroup(new Uri(builder.Configuration["Services:FindingsService"] + "/health"), "Findings Service")
//     // .AddUrlGroup(new Uri(builder.Configuration["Services:CompanyService"] + "/health"), "Company Service")
//     // .AddRedis(builder.Configuration.GetConnectionString("Redis"))
//     // .AddApplicationInsightsPublisher();

// Redis for caching and distributed rate limiting
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "FindingsGateway:";
});

// Memory Cache for local caching
builder.Services.AddMemoryCache();

// Distributed Cache for data loaders
builder.Services.AddDistributedMemoryCache();

// Application Insights
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    options.EnableAdaptiveSampling = false;
    options.EnablePerformanceCounterCollectionModule = true;
});

// Custom Services
builder.Services.AddScoped<IFindingService, FindingService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ISiteService, SiteService>();
builder.Services.AddSingleton<CorrelationIdHandler>();
builder.Services.AddSingleton<ICorrelationIdProvider, CorrelationIdProvider>();

// Swagger/OpenAPI
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo 
//     { 
//         Title = "Findings API Gateway", 
//         Version = "v1",
//         Description = "GraphQL Gateway for Findings Management System"
//     });
    
//     c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//     {
//         Description = "JWT Authorization header",
//         Name = "Authorization",
//         In = ParameterLocation.Header,
//         Type = SecuritySchemeType.Http,
//         Scheme = "bearer"
//     });
    
//     // c.OperationFilter<AddCorrelationIdHeader>();
// });

var app = builder.Build();

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI(c =>
    // {
    //     c.SwaggerEndpoint("/swagger/v1/swagger.json", "Findings Gateway V1");
    //     c.RoutePrefix = "api-docs";
    // });
    
    // GraphQL Playground/Altair
    // app.UseGraphQLPlayground("/playground", new PlaygroundOptions
    // {
    //     SchemaPollingEnabled = false,
    //     SchemaPollingInterval = 60000,
    //     EnableSubscription = true
    // });
    
    // app.UseGraphQLAltair("/altair");
}

// app.UseHttpsRedirection(); // Disabled for development
app.UseCors("GatewayCors");
// app.UseRateLimiter();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapControllers();
app.MapGraphQL("/graphql").RequireAuthorization();
app.MapGraphQLSchema("/graphql/schema");
// app.MapGraphQLVoyager("/voyager");
// app.MapBananaCakePop("/graphql-ui");
// app.MapHealthChecks("/health", new HealthCheckOptions
// {
//     ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
//     AllowCachingResponses = false
// });

// WebSocket support for GraphQL subscriptions
app.UseWebSockets();

// REST endpoints (for backward compatibility)
app.MapGet("/api/findings", async (HttpContext context, [FromServices] IFindingService service) =>
{
    var companyId = context.Request.Query["companyId"].FirstOrDefault();
    var includeCompany = bool.Parse(context.Request.Query["includeCompany"].FirstOrDefault() ?? "false");
    
    var query = new GetFindingsQuery
    {
        CompanyId = int.Parse(companyId ?? "0"),
        IncludeCompany = includeCompany
    };
    
    var findings = await service.GetFindingsAsync(query);
    
    return Results.Ok(findings);
}).RequireAuthorization("CanViewFindings");

app.MapGet("/api/findings/{id}", async (int id, [FromServices] IFindingService service) =>
{
    var finding = await service.GetFindingByIdAsync(id);
    return finding != null ? Results.Ok(finding) : Results.NotFound();
}).RequireAuthorization("CanViewFindings");

// Schema stitching endpoint (for federated GraphQL)
app.MapGet("/graphql/sdl", () =>
{
    var schema = app.Services.GetRequiredService<ISchema>();
    return schema.Print();
});

app.Run();

// Polly Retry Policy
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) + Random.Shared.NextDouble()));
}
