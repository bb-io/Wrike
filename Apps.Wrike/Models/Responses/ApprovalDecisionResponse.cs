namespace Apps.Wrike.Models.Responses;

public class ApprovalDecisionResponse
{
    [Display("Approver ID")]
    [JsonProperty("approverId")]
    public string ApproverId { get; set; } = string.Empty;

    [Display("Status")]
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [Display("Comment")]
    [JsonProperty("comment")]
    public string? Comment { get; set; }

    [Display("Updated date")]
    [JsonProperty("updatedDate")]
    public DateTime? UpdatedDate { get; set; }
}
