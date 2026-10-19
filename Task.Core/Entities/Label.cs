namespace TaskManager.Core.Entities;

// EN-03
public class Label
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // EN-03-R2: navigation only; many-to-many through TaskItemLabel
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
