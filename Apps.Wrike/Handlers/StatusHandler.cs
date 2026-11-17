namespace Apps.Wrike.Handlers;

public class StatusHandler(InvocationContext invocationContext, [ActionParameter] WorkflowIdentifier parent) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(parent.SpaceId) || string.IsNullOrWhiteSpace(parent.WorkflowId))
            return [];

        var items = await Client.Paginate<WorkflowResponse>(() => new RestRequest($"spaces/{InputValidator.Id(parent.SpaceId, "Space ID")}/workflows"), false, 10000, true);
        var account = await Client.Paginate<WorkflowResponse>(() => new RestRequest("workflows"), false, 10000, true);
        return items.Concat(account).Where(x => x.WorkflowId == parent.WorkflowId).SelectMany(x => x.CustomStatuses)
            .Where(x => !x.Hidden && (string.IsNullOrWhiteSpace(context.SearchString) || x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase) || x.CustomStatusId.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase)))
            .DistinctBy(x => x.CustomStatusId)
            .Select(x => new DataSourceItem(x.CustomStatusId, $"{x.Name} · {x.Group} [{x.CustomStatusId}]"));
    }
}
