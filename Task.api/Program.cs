using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using TaskManager.Api.Auth;
using TaskManager.Api.Extensions;
using TaskManager.Api.Middleware;
using TaskManager.Api.Options;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Data;
using TaskManager.Data.Repositories;
using TaskManager.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// GN-06-R2: NLog (nlog.config) behind the regular ILogger; levels per category from Logging:LogLevel
builder.Logging.ClearProviders();
builder.Host.UseNLog(new NLogAspNetCoreOptions { RemoveLoggerFactoryFilter = false });

// DP-01-R4: Render sets PORT; without it the container image listens on 8080
if (Environment.GetEnvironmentVariable("PORT") is { Length: > 0 } port)
    builder.WebHost.UseUrls($"http://+:{port}");

// AP-05; the connection string comes from User Secrets in development (IN-01)
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// AP-05: Repositories and Services are Scoped, like the DbContext they share.
// Mapping needs no registration: Mapperly mappers are static (deviation from SV-01, AP-05).
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ILabelRepository, LabelRepository>();
builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ILabelService, LabelService>();
builder.Services.AddScoped<ITaskService, TaskService>();
// The hasher behind the seed hashes (GN-05-R1); stateless, so one instance
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddSingleton(TimeProvider.System);

// IN-02, GN-04-R2, GN-08-R2: settings through IOptions; a missing value fails at startup.
// Environment variables override appsettings (Jwt__Key, Cors__ClientUrl, DP-04-R1).
builder.Services.AddOptions<JwtOptions>().Bind(builder.Configuration.GetSection(JwtOptions.Section))
    .ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<ClientCorsOptions>().Bind(builder.Configuration.GetSection(ClientCorsOptions.Section))
    .ValidateDataAnnotations().ValidateOnStart();
// GN-04-R1, AP-04
builder.Services.AddSingleton<JwtTokenFactory>();

// GN-04, MW-03: validate signature, issuer, audience and lifetime; no grace period, so the token expires after exactly 60 minutes
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme).Configure<IOptions<JwtOptions>>((bearer, jwt) =>
    bearer.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwt.Value.Issuer,
        ValidAudience = jwt.Value.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Value.Key)),
        ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
        ClockSkew = TimeSpan.Zero,
    });
// MW-04, FN-00-R1: every endpoint needs a valid token unless it allows anonymous access (only auth); roles per action (RL-01 to RL-04)
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

// GN-08: only the React client; all four methods, and the Authorization header (plus Content-Type for JSON bodies)
builder.Services.AddCors();
builder.Services.AddOptions<CorsOptions>().Configure<IOptions<ClientCorsOptions>>((cors, client) =>
    cors.AddPolicy(ClientCorsPolicy, policy => policy
        .WithOrigins(client.Value.ClientUrl.TrimEnd('/'))
        .WithMethods("GET", "POST", "PUT", "DELETE")
        .WithHeaders("Authorization", "Content-Type")));

// DT-00-R5, CO-02: enums as strings ("Open", "Admin")
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// GN-03, GN-07: ProblemDetails with Hebrew titles and correlationId
builder.Services.AddApiProblemDetails();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// GN-10-R2: an Authorize button for a Bearer token
builder.Services.AddSwaggerGen(options =>
{
    var bearer = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "The token from POST /api/auth/login",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
    };
    options.AddSecurityDefinition("Bearer", bearer);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { [bearer] = [] });
});

var app = builder.Build();

// DP-04-R3, RD-06: apply the migrations, seed included, when the server starts. Tests turn it off and recreate their own database.
if (app.Configuration.GetValue("Database:MigrateOnStartup", true))
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<TaskDbContext>().Database.Migrate();
}

// GN-01 order: errors -> CorrelationId -> CORS -> Authentication -> Authorization
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
// GN-03-R2: an empty 401/403 from Authentication/Authorization becomes the same ProblemDetails
app.UseStatusCodePages();

// GN-10-R1: development only
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// GN-08-R4: before Authentication, so a preflight OPTIONS without a token passes
app.UseCors(ClientCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
    private const string ClientCorsPolicy = "Client";
}
