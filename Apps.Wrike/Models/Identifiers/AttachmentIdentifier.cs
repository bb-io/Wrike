namespace Apps.Wrike.Models.Identifiers;

public class AttachmentIdentifier : TaskChildIdentifier
{
    [Display("Attachment ID")]
    [DataSource(typeof(AttachmentHandler))]
    public string AttachmentId { get; set; } = string.Empty;
}
