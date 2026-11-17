namespace Apps.Wrike.Models.Responses;

public class ApprovalResponse
{
    [Display("Approval ID")]
    [JsonProperty("id")]
    public string ApprovalId { get; set; } = string.Empty;

    [Display("Title")]
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [Display("Description")]
    [JsonProperty("description")]
    public string? Description { get; set; }

    [Display("Status")]
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [Display("Task ID")]
    [JsonProperty("taskId")]
    public string? TaskId { get; set; }

    [Display("Due date")]
    [JsonProperty("dueDate")]
    public DateTime? DueDate { get; set; }

    [Display("Attachment IDs")]
    [JsonProperty("attachmentIds")]
    public List<string> AttachmentIds { get; set; } = [];
    [Display("Decisions")]
    [JsonProperty("decisions")]
    public List<ApprovalDecisionResponse> Decisions { get; set; } = [];
}
