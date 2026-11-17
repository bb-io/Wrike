namespace Apps.Wrike.Models.Requests;

public class BulkTaskRequest : WorkflowIdentifier
{
    [Display("Task IDs")]
    [DataSource(typeof(TaskHandler))]
    public List<string> TaskIds { get; set; } = [];

    [Display("Custom status ID")]
    [DataSource(typeof(StatusHandler))]
    public string? CustomStatusId { get; set; }

    [Display("Add responsible IDs")]
    [DataSource(typeof(UserHandler))]
    public List<string>? AddResponsibleIds { get; set; }

    [Display("Remove responsible IDs")]
    [DataSource(typeof(UserHandler))]
    public List<string>? RemoveResponsibleIds { get; set; }

    [Display("Custom fields JSON")]
    public string? CustomFieldsJson { get; set; }
}
