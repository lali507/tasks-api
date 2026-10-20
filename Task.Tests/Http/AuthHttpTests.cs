using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Enums;
using TaskManager.Core.Results;
using TaskManager.Tests.Infrastructure;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace TaskManager.Tests.Http;

[Collection(DatabaseCollection.Name)]
public class AuthHttpTests(DatabaseFixture fixture) : HttpTestBase(fixture)
{
    private async Task DeleteUserAsync(string email)
    {
        var users = await ReadAsync<PagedResponse<UserResponse>>(await Admin.GetAsync("/api/users?pageSize=50"));
        var user = users.Items.Single(u => u.Email == email);
        Assert.Equal(HttpStatusCode.NoContent, (await Admin.DeleteAsync($"/api/users/{user.Id}")).StatusCode);
    }

    // TC-A-01
    [Fact]
    public async Task Register_Valid_Returns201WithToken()
    {
        var email = $"{Guid.NewGuid():N}@test.com";

        var auth = await ReadAsync<AuthResponse>(
            await Anonymous.PostAsJsonAsync("/api/auth/register", new { email, displayName = "New Member", password = "Password1" }), HttpStatusCode.Created);

        Assert.False(string.IsNullOrEmpty(auth.Token));
        Assert.Equal("New Member", auth.DisplayName);
        Assert.Equal(Role.Member, auth.Role);
        await DeleteUserAsync(email);
    }

    // TC-A-06, RL-01: a Role field in the body is ignored
    [Fact]
    public async Task Register_WithRoleAdminInBody_IsMember()
    {
        var email = $"{Guid.NewGuid():N}@test.com";

        var auth = await ReadAsync<AuthResponse>(
            await Anonymous.PostAsJsonAsync("/api/auth/register", new { email, displayName = "Sneaky", password = "Password1", role = "Admin" }), HttpStatusCode.Created);

        Assert.Equal(Role.Member, auth.Role);
        await DeleteUserAsync(email);
    }

    // TC-A-07, GN-04: claims sub, role, email; 60 minutes
    [Fact]
    public async Task Login_SeedUser_TokenHasClaims()
    {
        var auth = await ReadAsync<AuthResponse>(
            await Anonymous.PostAsJsonAsync("/api/auth/login", new { email = "member02@example.com", password = "Demo1234!2" }));

        var token = new JsonWebToken(auth.Token);
        Assert.Equal("2", token.Subject);
        Assert.Equal("Member", token.GetClaim("role").Value);
        Assert.Equal("member02@example.com", token.GetClaim(JwtRegisteredClaimNames.Email).Value);
        Assert.Equal(TimeSpan.FromMinutes(60), token.ValidTo - token.IssuedAt);
    }

    // TC-A-11: the first request after expiry
    [Fact]
    public async Task ExpiredToken_Returns401()
    {
        var past = DateTime.UtcNow.AddHours(-2);
        var expired = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([new Claim(JwtRegisteredClaimNames.Sub, "2"), new Claim("role", "Member")]),
            Issuer = "TaskManager.Api",
            Audience = "TaskManager.Client",
            NotBefore = past,
            IssuedAt = past,
            Expires = past.AddMinutes(60),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiFactory.TestJwtKey)), SecurityAlgorithms.HmacSha256),
        });
        var client = Anonymous;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", expired);

        await Problem.AssertAsync(await client.GetAsync("/api/projects"), HttpStatusCode.Unauthorized, ErrorMessages.LoginRequired);
    }

    // TC-A-12, GN-06-R1: no password or token in the log
    [Fact]
    public async Task RegisterAndLogin_LogHasNoPasswordOrToken()
    {
        var email = $"{Guid.NewGuid():N}@test.com";
        const string password = "Secret987xyz";
        Api.Logs.Clear();

        var auth = await ReadAsync<AuthResponse>(
            await Anonymous.PostAsJsonAsync("/api/auth/register", new { email, displayName = "Log Check", password }), HttpStatusCode.Created);
        await Anonymous.PostAsJsonAsync("/api/auth/login", new { email, password });

        var log = string.Join("\n", Api.Logs.Entries.Select(e => e.Message + e.Exception));
        Assert.Contains("POST /api/auth/register responded 201", log);
        Assert.DoesNotContain(password, log);
        Assert.DoesNotContain(auth.Token, log);
        await DeleteUserAsync(email);
    }
}
