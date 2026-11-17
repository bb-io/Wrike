namespace Apps.Wrike.Handlers;

public class ApprovalHandler(InvocationContext invocationContext, [ActionParameter] TaskIdentifier parent) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(parent.TaskId))
            return [];

        var items = await Client.Paginate<ApprovalResponse>(() => new RestRequest($"tasks/{InputValidator.Id(parent.TaskId, "Task ID")}/approvals"), false, 10000, true);
        return items.Select(x => new DataSourceItem(x.ApprovalId, $"{(x.Title + " · " + x.Status + " · " + x.Description)} [{x.ApprovalId}]"))
            .Where(x => string.IsNullOrWhiteSpace(context.SearchString) || x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase) || x.Value.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(100);
    }
}
