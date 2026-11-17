namespace Apps.Wrike.Handlers;

public class SpaceHandler(InvocationContext invocationContext) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var items = await Client.Paginate<SpaceResponse>(() => new RestRequest("spaces").Optional("title", context.SearchString), false, 10000, true);
        return items.Select(x => new DataSourceItem(x.SpaceId, $"{(x.Title)} [{x.SpaceId}]"))
            .Where(x => string.IsNullOrWhiteSpace(context.SearchString) || x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase) || x.Value.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(100);
    }
}
