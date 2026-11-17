namespace Apps.Wrike.Models.Identifiers;

public class TaskChildIdentifier : SpaceIdentifier
{
    [Display("Task ID", Description = "Select a task to discover related items; optional when mapping the item ID directly.")]
    [DataSource(typeof(TaskHandler))]
    public string? TaskId { get; set; }
}
