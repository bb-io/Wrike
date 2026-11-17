namespace Apps.Wrike.Models.Responses;

public class SearchCommentResponse
{
    [Display("Comments")]
    public List<CommentResponse> Comments { get; set; } = [];
}
