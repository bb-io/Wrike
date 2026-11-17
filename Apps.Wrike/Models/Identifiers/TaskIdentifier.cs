namespace Apps.Wrike.Models.Identifiers;

public class TaskIdentifier : SpaceIdentifier
{
    [Display("Task ID")]
    [DataSource(typeof(TaskHandler))]
    public string TaskId { get; set; } = string.Empty;
}
