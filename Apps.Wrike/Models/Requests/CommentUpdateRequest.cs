namespace Apps.Wrike.Models.Requests;

public class CommentUpdateRequest : CommentIdentifier
{
    [Display("Text")]
    public string Text { get; set; } = string.Empty;

    [Display("Plain text")]
    public bool? PlainText { get; set; }
}
