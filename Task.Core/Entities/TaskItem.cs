using TaskManager.Core.Enums;

namespace TaskManager.Core.Entities;

// EN-04
public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Status Status { get; set; } = Status.Open;
    public int ProjectId { get; set; }
    public int? TakenByUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    // EN-04-R5: added in migration 2
    public DateTime? CompletedAt { get; set; }

    // EN-04-R3: PostgreSQL system column, mapped as a concurrency token in Data
    public uint Xmin { get; set; }

    // EN-04-R1: navigation only
    public Project Project { get; set; } = null!;
    public User? TakenByUser { get; set; }
    public ICollection<Label> Labels { get; set; } = new List<Label>();
}
