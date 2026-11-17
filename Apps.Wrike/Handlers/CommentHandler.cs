namespace Apps.Wrike.Handlers;

public class CommentHandler(InvocationContext invocationContext, [ActionParameter] TaskIdentifier parent) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(parent.TaskId))
            return [];

        var items = await Client.Paginate<CommentResponse>(() => new RestRequest($"tasks/{InputValidator.Id(parent.TaskId, "Task ID")}/comments").Optional("plainText", true), false, 10000, true);
        return items.Select(x => new DataSourceItem(x.CommentId, $"{(x.Text.Length > 100 ? x.Text[..100] : x.Text)} [{x.CommentId}]"))
            .Where(x => string.IsNullOrWhiteSpace(context.SearchString) || x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase) || x.Value.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(100);
    }
}
