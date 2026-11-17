namespace Apps.Wrike.Handlers;

public class WorkflowHandler(InvocationContext invocationContext, [ActionParameter] SpaceIdentifier parent) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(parent.SpaceId))
            return [];

        var items = await Client.Paginate<WorkflowResponse>(() => new RestRequest($"spaces/{InputValidator.Id(parent.SpaceId, "Space ID")}/workflows"), false, 10000, true);
        var account = await Client.Paginate<WorkflowResponse>(() => new RestRequest("workflows"), false, 10000, true);
        return items.Concat(account).DistinctBy(x => x.WorkflowId)
            .Where(x => string.IsNullOrWhiteSpace(context.SearchString) || x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase) || x.WorkflowId.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x.WorkflowId, $"{x.Name} [{x.WorkflowId}]"));
    }
}
