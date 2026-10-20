using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Api.Auth;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Enums;

namespace TaskManager.Tests.Infrastructure;

// The API in memory (Development). Points it at the test database and a test-only signing key.
public class ApiFactory : WebApplicationFactory<Program>
{
    public const string TestJwtKey = "test-only-signing-key-that-is-long-enough-for-hs256";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = TestDatabase.ConnectionString,
            ["Jwt:Key"] = TestJwtKey,
        }));
        builder.ConfigureServices(ConfigureTestServices);
    }

    // Controller tests replace the Services with mocks here
    protected virtual void ConfigureTestServices(IServiceCollection services) { }

    // A client with a token for this user, created the same way the API creates it (GN-04)
    public HttpClient CreateClientAs(int userId, Role role, string email = "user@example.com")
    {
        var client = CreateClient();
        var token = Services.GetRequiredService<JwtTokenFactory>()
            .Create(new UserResponse { Id = userId, Role = role, Email = email, DisplayName = "Test" });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
