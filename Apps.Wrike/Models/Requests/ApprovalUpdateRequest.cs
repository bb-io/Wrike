namespace Apps.Wrike.Models.Requests;

public class ApprovalUpdateRequest : ApprovalIdentifier
{
    [Display("Description")]
    public string? Description { get; set; }

    [Display("Due date")]
    public DateTime? DueDate { get; set; }

    [Display("Add approver IDs")]
    [DataSource(typeof(UserHandler))]
    public List<string>? AddApproverIds { get; set; }

    [Display("Remove approver IDs")]
    [DataSource(typeof(UserHandler))]
    public List<string>? RemoveApproverIds { get; set; }

    [Display("Add attachment IDs")]
    [DataSource(typeof(AttachmentHandler))]
    public List<string>? AddAttachmentIds { get; set; }

    [Display("Remove attachment IDs")]
    [DataSource(typeof(AttachmentHandler))]
    public List<string>? RemoveAttachmentIds { get; set; }
}
