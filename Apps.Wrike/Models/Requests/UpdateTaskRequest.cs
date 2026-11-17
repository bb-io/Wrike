namespace Apps.Wrike.Models.Requests;

public class UpdateTaskRequest : WorkflowIdentifier
{
    [Display("Task ID", Description = "Search accessible tasks by name. Optionally select Space ID to narrow suggestions. Manual and mapped task IDs are accepted.")]
    [DataSource(typeof(TaskHandler))]
    public string TaskId { get; set; } = string.Empty;

    [Display("Title")]
    public string? Title { get; set; }

    [Display("Description")]
    public string? Description { get; set; }

    [Display("Status")]
    [StaticDataSource(typeof(TaskStatusHandler))]
    public string? Status { get; set; }

    [Display("Custom status ID")]
    [DataSource(typeof(StatusHandler))]
    public string? CustomStatusId { get; set; }

    [Display("Start date")]
    public DateTime? StartDate { get; set; }

    [Display("Due date")]
    public DateTime? DueDate { get; set; }

    [Display("Metadata JSON", Description = "JSON array of key/value objects. Omit to preserve metadata.")]
    public string? MetadataJson { get; set; }

    [Display("Add responsible IDs")]
    [DataSource(typeof(UserHandler))]
    public List<string>? AddResponsibleIds { get; set; }

    [Display("Remove responsible IDs")]
    [DataSource(typeof(UserHandler))]
    public List<string>? RemoveResponsibleIds { get; set; }
}
