namespace Apps.Wrike.Models.Responses;

public class WorkflowResponse
{
    [Display("Workflow ID")]
    [JsonProperty("id")]
    public string WorkflowId { get; set; } = string.Empty;

    [Display("Name")]
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [Display("Custom statuses")]
    [JsonProperty("customStatuses")]
    public List<StatusResponse> CustomStatuses { get; set; } = [];
}
