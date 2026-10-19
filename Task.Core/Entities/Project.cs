namespace TaskManager.Core.Entities;

// EN-02
public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    // EN-02-R2: navigation only; stored in TaskItem.ProjectId
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
