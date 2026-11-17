namespace Apps.Wrike.Handlers;

public class GroupHandler(InvocationContext invocationContext) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var items = await Client.Paginate<GroupResponse>(() => new RestRequest("groups"), true, 10000, true, 100, "pageToken");
        return items.Select(x => new DataSourceItem(x.GroupId, $"{(x.Title)} [{x.GroupId}]"))
            .Where(x => string.IsNullOrWhiteSpace(context.SearchString) || x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase) || x.Value.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(100);
    }
}
