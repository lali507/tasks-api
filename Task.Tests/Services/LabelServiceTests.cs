using Moq;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Results;
using TaskManager.Service.Services;

namespace TaskManager.Tests.Services;

public class LabelServiceTests
{
    private const int AdminId = 1;
    private readonly Mock<ILabelRepository> _labels = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly LabelService _service;

    public LabelServiceTests()
    {
        _users.Setup(u => u.ExistsAsync(AdminId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _labels.Setup(l => l.AddAsync(It.IsAny<Label>(), It.IsAny<CancellationToken>())).ReturnsAsync(WriteResult.Success);
        _labels.Setup(l => l.UpdateAsync(It.IsAny<Label>(), It.IsAny<CancellationToken>())).ReturnsAsync(WriteResult.Success);
        _service = new LabelService(_labels.Object, _users.Object);
    }

    // TC-L-04
    [Fact]
    public async Task Create_ExistingName_ReturnsConflict()
    {
        _labels.Setup(l => l.NameExistsAsync("BUG", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.CreateAsync(AdminId, new LabelRequest { Name = " BUG " }, default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(ErrorMessages.LabelNameTaken, result.Title);
    }

    // GN-12
    [Fact]
    public async Task Create_DuplicateFromRepository_ReturnsConflict()
    {
        _labels.Setup(l => l.AddAsync(It.IsAny<Label>(), It.IsAny<CancellationToken>())).ReturnsAsync(WriteResult.Duplicate);

        var result = await _service.CreateAsync(AdminId, new LabelRequest { Name = "New label" }, default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(ErrorMessages.LabelNameTaken, result.Title);
    }

    // TC-L-08
    [Fact]
    public async Task Update_NameOfAnotherLabel_ReturnsConflict()
    {
        _labels.Setup(l => l.FindAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(new Label { Id = 3, Name = "Urgent" });
        _labels.Setup(l => l.NameExistsAsync("Bug", 3, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.UpdateAsync(AdminId, 3, new LabelRequest { Name = "Bug" }, default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(ErrorMessages.LabelNameTaken, result.Title);
    }

    [Fact]
    public async Task Update_MissingLabel_ReturnsNotFound()
    {
        var result = await _service.UpdateAsync(AdminId, 999, new LabelRequest { Name = "Bug" }, default);

        Assert.Equal(ResultStatus.NotFound, result.Status);
    }
}
