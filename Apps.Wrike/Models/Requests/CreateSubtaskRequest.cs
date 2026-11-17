namespace Apps.Wrike.Models.Requests;

public class CreateSubtaskRequest : CreateTaskRequest
{
    [Display("Parent task ID")]
    [DataSource(typeof(TaskHandler))]
    public string ParentTaskId { get; set; } = string.Empty;
}
