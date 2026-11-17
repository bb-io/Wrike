namespace Apps.Wrike.Handlers;

public class RequestFormHandler(InvocationContext invocationContext, [ActionParameter] SpaceIdentifier parent) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(parent.SpaceId))
            return [];

        var items = await Client.Paginate<RequestFormResponse>(() => new RestRequest($"spaces/{InputValidator.Id(parent.SpaceId, "Space ID")}/request_forms"), true, 10000, true, 100);
        return items.Select(x => new DataSourceItem(x.RequestFormId, $"{(x.Title)} [{x.RequestFormId}]"))
            .Where(x => string.IsNullOrWhiteSpace(context.SearchString) || x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase) || x.Value.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(100);
    }
}
