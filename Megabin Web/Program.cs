using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.PostgreSql;
using Megabin_Web.Features.Address;
using Megabin_Web.Features.Admin;
using Megabin_Web.Features.Auth;
using Megabin_Web.Features.Customer;
using Megabin_Web.Features.DriverDashboard;
using Megabin_Web.Features.RouteOptimization;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.Infrastructure.APILimitationService;
using Megabin_Web.Shared.Infrastructure.CurrentUserService;
using Megabin_Web.Shared.Infrastructure.HangfireAuthorizationFilter;
using Megabin_Web.Shared.Infrastructure.JWTTokenService;
using Megabin_Web.Shared.Infrastructure.MapBoxService;
using Megabin_Web.Shared.Infrastructure.OpenRouteService;
using Megabin_Web.Shared.Infrastructure.PasswordService;
using Megabin_Web.Shared.Infrastructure.WhatsAppService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Megabin API", Version = "v1" });

    // Add server URLs for API client generation
    opt.AddServer(
        new OpenApiServer { Url = "https://localhost:7012", Description = "Development HTTPS" }
    );
    opt.AddServer(
        new OpenApiServer { Url = "http://localhost:5250", Description = "Development HTTP" }
    );

    // Add schema filter to convert enums to strings (matching runtime JSON serialization)
    opt.SchemaFilter<Megabin_Web.Shared.Infrastructure.Swagger.EnumSchemaFilter>();

    opt.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer",
        }
    );

    opt.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                new string[] { }
            },
        }
    );
});

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Hangfire - Background job processing
builder.Services.AddHangfire(configuration =>
    configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options =>
            options.UseNpgsqlConnection(
                builder.Configuration.GetConnectionString("DefaultConnection")
            )
        )
);

builder.Services.AddHangfireServer();

// Configs.
builder.Services.Configure<OpenRouteServiceOptions>(
    builder.Configuration.GetSection("OpenRouteService")
);

builder.Services.Configure<MapboxOptions>(builder.Configuration.GetSection("Mapbox"));
builder.Services.Configure<APILimitOptions>(builder.Configuration.GetSection("APILimits"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<WhatsAppOptions>(builder.Configuration.GetSection("WhatsApp"));

var jwtOptions =
    builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration is missing");

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
    });

builder.Services.AddAuthorization();

// CORS - Allow all origins
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});

// HTTP clients for external services
builder.Services.AddHttpClient<IRouteOptimizationService, RouteOptimizationService>();
builder.Services.AddHttpClient<IMapboxService, MapboxService>();
builder.Services.AddHttpClient<IWhatsAppService, WhatsAppService>();

// Application services
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAPILimitationService, APILimitationService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddMediatR(cfg =>
{
    cfg.LicenseKey = builder.Configuration.GetSection("MediatR:LicenseKey").Value;
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);

    // Register pipeline behaviors (order matters - they execute in registration order)
    cfg.AddOpenBehavior(typeof(Megabin_Web.Shared.Infrastructure.MediatR.LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(Megabin_Web.Shared.Infrastructure.MediatR.ErrorHandlingBehavior<,>));
});

// Background jobs
builder.Services.AddScoped<RouteOptimizationBackgroundJob>();

// Configure JSON serialization for minimal APIs
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter(namingPolicy: JsonNamingPolicy.CamelCase)
    );
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Run database migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Megabin API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
}

// Global exception handling middleware (should be first)
app.UseMiddleware<Megabin_Web.Shared.Infrastructure.Middleware.GlobalExceptionHandlingMiddleware>();

// Hangfire Dashboard
app.UseHangfireDashboard(
    "/hangfire",
    new DashboardOptions { Authorization = new[] { new HangfireAuthorizationFilter() } }
);

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Map feature endpoints
app.MapAddressEndpoints();
app.MapAdminEndpoints();
app.MapAuthEndpoints();
app.MapCustomerEndpoints();
app.MapDriverEndpoints();
app.MapRouteOptimizationEndpoints();

// Configure recurring jobs
RecurringJob.AddOrUpdate<RouteOptimizationBackgroundJob>(
    "daily-route-optimization",
    job => job.OptimizeRoutesAsync(),
    Cron.Daily(2) // Run daily at 2 AM
);

app.Run();
