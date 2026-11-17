namespace Apps.Wrike.Models.Responses;

public class FormSubmitResponse
{
    [Display("Request form ID")]
    [JsonProperty("formId")]
    public string RequestFormId { get; set; } = string.Empty;

    [Display("Task ID")]
    [JsonProperty("taskId")]
    public string? TaskId { get; set; }

    [Display("Project ID")]
    [JsonProperty("projectId")]
    public string? ProjectId { get; set; }
}
