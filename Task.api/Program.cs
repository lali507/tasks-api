using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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

// Add services to the container.

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
builder.Services.AddSwaggerGen();

var app = builder.Build();

// GN-01 order: errors -> CorrelationId -> CORS -> Authentication -> Authorization
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// GN-08-R4: before Authentication, so a preflight OPTIONS without a token passes
app.UseCors(ClientCorsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
    private const string ClientCorsPolicy = "Client";
}
