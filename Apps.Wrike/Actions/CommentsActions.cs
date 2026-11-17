namespace Apps.Wrike.Actions;

[ActionList("Comments")]
public class CommentsActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Search comments", Description = "Search comments.")]
    public async Task<SearchCommentResponse> SearchComments([ActionParameter] TaskIdentifier input)
    {
        return new() { Comments = await Client.Paginate<CommentResponse>(() => new RestRequest($"tasks/{InputValidator.Id(input.TaskId, "Task ID")}/comments").Optional("plainText", true), false) };
    }

    [Action("Create comment", Description = "Create comment.")]
    public async Task<CommentResponse> CreateComment([ActionParameter] CommentCreateRequest input)
    {
        InputValidator.Required(input.Text, "Text");
        var result = await Client.ExecuteWithErrorHandling<Envelope<CommentResponse>>(new RestRequest($"tasks/{InputValidator.Id(input.TaskId, "Task ID")}/comments", Method.Post).Optional("text", input.Text).Optional("plainText", input.PlainText ?? true));
        return result.Data.First();
    }

    [Action("Update comment", Description = "Update comment.")]
    public async Task<CommentResponse> UpdateComment([ActionParameter] CommentUpdateRequest input)
    {
        InputValidator.Required(input.Text, "Text");
        var result = await Client.ExecuteWithErrorHandling<Envelope<CommentResponse>>(new RestRequest($"comments/{InputValidator.Id(input.CommentId, "Comment ID")}", Method.Put).Optional("text", input.Text).Optional("plainText", input.PlainText ?? true));
        return result.Data.First();
    }

    [Action("Delete comment", Description = "Delete comment.")]
    public async Task DeleteComment([ActionParameter] CommentIdentifier input)
    {
        await Client.ExecuteWithErrorHandling(new RestRequest($"comments/{InputValidator.Id(input.CommentId, "Comment ID")}", Method.Delete));
    }
}
