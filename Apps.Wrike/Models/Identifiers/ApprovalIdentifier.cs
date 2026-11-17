namespace Apps.Wrike.Models.Identifiers;

public class ApprovalIdentifier : TaskChildIdentifier
{
    [Display("Approval ID")]
    [DataSource(typeof(ApprovalHandler))]
    public string ApprovalId { get; set; } = string.Empty;
}
