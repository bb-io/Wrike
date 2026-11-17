namespace Apps.Wrike.Models.Identifiers;

public class CommentIdentifier : TaskChildIdentifier
{
    [Display("Comment ID")]
    [DataSource(typeof(CommentHandler))]
    public string CommentId { get; set; } = string.Empty;
}
