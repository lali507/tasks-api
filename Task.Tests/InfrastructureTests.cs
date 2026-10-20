using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Tests.Infrastructure;

namespace TaskManager.Tests;

// Task 36: the infrastructure works against PostgreSQL
[Collection(DatabaseCollection.Name)]
public class InfrastructureTests(DatabaseFixture fixture)
{
    // GN-05-R3: the recreated test database has the same seed as development
    [Fact]
    public async Task TestDatabase_IsRecreatedWithSeed()
    {
        await using var db = TestDatabase.CreateContext();

        Assert.Equal(30, await db.Users.CountAsync());
        Assert.Equal(10, await db.Projects.CountAsync());
        Assert.Equal(10, await db.Labels.CountAsync());
        Assert.Equal(100, await db.TaskItems.CountAsync());
    }

    // The in-memory API reaches the test database
    [Fact]
    public async Task Api_LoginWithSeedUser_ReturnsToken()
    {
        var client = fixture.Api.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Email = "admin@example.com", Password = "Demo1234!1" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>(Json.Options);
        Assert.False(string.IsNullOrEmpty(auth!.Token));
    }
}
