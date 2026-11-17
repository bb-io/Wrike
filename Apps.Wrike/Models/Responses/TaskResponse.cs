namespace Apps.Wrike.Models.Responses;

public class TaskResponse
{
    [Display("Task ID")]
    [JsonProperty("id")]
    public string TaskId { get; set; } = string.Empty;

    [Display("Title")]
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [Display("Description")]
    [JsonProperty("description")]
    public string? Description { get; set; }

    [Display("Status")]
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [Display("Custom status ID")]
    [JsonProperty("customStatusId")]
    public string? CustomStatusId { get; set; }

    [Display("Permalink")]
    [JsonProperty("permalink")]
    public string Permalink { get; set; } = string.Empty;

    [Display("Parent IDs")]
    [JsonProperty("parentIds")]
    public List<string> ParentIds { get; set; } = [];

    [Display("Super task IDs")]
    [JsonProperty("superTaskIds")]
    public List<string> SuperTaskIds { get; set; } = [];

    [Display("Sub task IDs")]
    [JsonProperty("subTaskIds")]
    public List<string> SubTaskIds { get; set; } = [];

    [Display("Responsible IDs")]
    [JsonProperty("responsibleIds")]
    public List<string> ResponsibleIds { get; set; } = [];

    [Display("Custom fields")]
    [JsonProperty("customFields")]
    public List<FieldValueResponse> CustomFields { get; set; } = [];

    [Display("Metadata")]
    [JsonProperty("metadata")]
    public List<MetadataResponse> Metadata { get; set; } = [];

    [Display("Dates")]
    [JsonProperty("dates")]
    public TaskDatesResponse? Dates { get; set; }

    [Display("Created date")]
    [JsonProperty("createdDate")]
    public DateTime? CreatedDate { get; set; }

    [Display("Updated date")]
    [JsonProperty("updatedDate")]
    public DateTime? UpdatedDate { get; set; }
}
