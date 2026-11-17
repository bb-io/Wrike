namespace Apps.Wrike.Models.Requests;

public class CreateTaskRequest : WorkflowIdentifier
{
    [Display("Folder ID", Description = "Search accessible folders by name. Optionally select Space ID to narrow suggestions. Manual and mapped folder IDs are accepted.")]
    [DataSource(typeof(FolderHandler))]
    public string FolderId { get; set; } = string.Empty;

    [Display("Title")]
    public string Title { get; set; } = string.Empty;

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

    [Display("Responsible IDs")]
    [DataSource(typeof(UserHandler))]
    public List<string>? ResponsibleIds { get; set; }
}
