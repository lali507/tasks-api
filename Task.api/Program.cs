using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Extensions;
using TaskManager.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// AP-05; the connection string comes from User Secrets in development (IN-01)
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// DT-00-R5, CO-02: enums as strings ("Open", "Admin")
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// GN-03, GN-07: ProblemDetails with Hebrew titles and correlationId
builder.Services.AddApiProblemDetails();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
