using TaskManager.Core.Enums;

namespace TaskManager.Core.Entities;

// EN-01
public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.Member;
    public DateTime CreatedAt { get; set; }

    // EN-01-R6: navigation only; stored in TaskItem.TakenByUserId
    public ICollection<TaskItem> TakenTasks { get; set; } = new List<TaskItem>();
}
