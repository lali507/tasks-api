using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using TaskManager.Core.Interfaces.Services;

namespace TaskManager.Tests.Infrastructure;

// Controller level (document 6): the real MVC pipeline (routing, auth, ModelState, result mapping) over mocked Services; no database
public class MockedServicesFactory : ApiFactory
{
    public Mock<IAuthService> Auth { get; } = new();
    public Mock<IProjectService> Projects { get; } = new();
    public Mock<ILabelService> Labels { get; } = new();
    public Mock<ITaskService> Tasks { get; } = new();
    public Mock<IUserService> Users { get; } = new();

    protected override void ConfigureTestServices(IServiceCollection services)
    {
        services.RemoveAll<IAuthService>().AddSingleton(Auth.Object);
        services.RemoveAll<IProjectService>().AddSingleton(Projects.Object);
        services.RemoveAll<ILabelService>().AddSingleton(Labels.Object);
        services.RemoveAll<ITaskService>().AddSingleton(Tasks.Object);
        services.RemoveAll<IUserService>().AddSingleton(Users.Object);
    }

    // The mocks are shared by the tests of a collection; each test starts clean
    public void ResetMocks()
    {
        Auth.Reset();
        Projects.Reset();
        Labels.Reset();
        Tasks.Reset();
        Users.Reset();
    }
}

[CollectionDefinition(Name)]
public class ControllerCollection : ICollectionFixture<MockedServicesFactory>
{
    public const string Name = "Controllers";
}
