namespace Apps.Wrike.Models.Requests;

public class CommentCreateRequest : TaskIdentifier
{
    [Display("Text")]
    public string Text { get; set; } = string.Empty;

    [Display("Plain text")]
    public bool? PlainText { get; set; }
}
