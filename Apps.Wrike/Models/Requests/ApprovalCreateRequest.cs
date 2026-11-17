namespace Apps.Wrike.Models.Requests;

public class ApprovalCreateRequest : TaskIdentifier
{
    [Display("Description")]
    public string? Description { get; set; }

    [Display("Due date")]
    public DateTime? DueDate { get; set; }

    [Display("Approver IDs")]
    [DataSource(typeof(UserHandler))]
    public List<string>? ApproverIds { get; set; }

    [Display("Attachment IDs")]
    [DataSource(typeof(AttachmentHandler))]
    public List<string>? AttachmentIds { get; set; }

    [Display("Auto finish on approve")]
    public bool? AutoFinishOnApprove { get; set; }

    [Display("Auto finish on reject")]
    public bool? AutoFinishOnReject { get; set; }
}
