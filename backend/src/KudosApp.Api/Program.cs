using System.Text;
using KudosApp.Api.Endpoints;
using KudosApp.Application;
using KudosApp.Domain.Entities;
using KudosApp.Infrastructure;
using KudosApp.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

// Load .env file BEFORE building host (maps OPENAI_API_KEY → OpenAI__ApiKey for .NET config)
var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".env");
if (File.Exists(envPath))
{
    foreach (var line in File.ReadAllLines(envPath))
    {
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#')) continue;
        var parts = line.Split('=', 2);
        if (parts.Length == 2)
            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
    }
    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    var model = Environment.GetEnvironmentVariable("OPENAI_MODEL");
    if (!string.IsNullOrWhiteSpace(apiKey))
        Environment.SetEnvironmentVariable("OpenAI__ApiKey", apiKey);
    if (!string.IsNullOrWhiteSpace(model))
        Environment.SetEnvironmentVariable("OpenAI__Model", model);
}

var builder = WebApplication.CreateBuilder(args);

// Application (services, validators)
builder.Services.AddApplication();

// Infrastructure (DbContext)
builder.Services.AddInfrastructure(builder.Configuration);

// Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("JWT Key is not configured. Set Jwt__Key environment variable.");

builder.Services
    .AddAuthentication(options =>
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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// OpenAPI
builder.Services.AddOpenApi();

// Response compression
builder.Services.AddResponseCompression(opts =>
{
    opts.EnableForHttps = true;
    opts.MimeTypes = Microsoft.AspNetCore.ResponseCompression.ResponseCompressionDefaults.MimeTypes;
});

// Output caching
builder.Services.AddOutputCache();

// CORS for Angular dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Ensure database directory exists and seed roles
using (var scope = app.Services.CreateScope())
{
    var connStr = scope.ServiceProvider.GetRequiredService<IConfiguration>()
        .GetConnectionString("DefaultConnection") ?? "";
    var dbFile = connStr.Replace("Data Source=", "");
    if (!string.IsNullOrEmpty(dbFile))
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(dbFile));
        if (dir is not null) Directory.CreateDirectory(dir);
    }

    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.EnsureCreatedAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    await DbInitializer.SeedAsync(roleManager, userManager, logger);
}

// Global exception handler — never expose stack traces
app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(async context =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>()?.Error;
        logger.LogError(exception, "Unhandled exception at {Path}", context.Request.Path);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(new
        {
            type = "https://tools.ietf.org/html/rfc7807",
            title = "An unexpected error occurred",
            status = 500,
            traceId = context.TraceIdentifier
        });
    });
});

app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseOutputCache();

// Minimal API endpoints
app.MapAuthEndpoints();
app.MapKudosEndpoints();
app.MapCategoriesEndpoints();
app.MapLeaderboardEndpoints();
app.MapNotificationsEndpoints();
app.MapUsersEndpoints();
app.MapAiEndpoints();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();
