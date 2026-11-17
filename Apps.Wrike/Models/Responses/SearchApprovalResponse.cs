namespace Apps.Wrike.Models.Responses;

public class SearchApprovalResponse
{
    [Display("Approvals")]
    public List<ApprovalResponse> Approvals { get; set; } = [];
}
