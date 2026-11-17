namespace Apps.Wrike.Handlers;

public class TimelogCategoryHandler(InvocationContext invocationContext) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var items = await Client.Paginate<TimelogCategoryResponse>(() => new RestRequest("timelog_categories"), false, 10000, true);
        return items.Select(x => new DataSourceItem(x.CategoryId, $"{(x.Name)} [{x.CategoryId}]"))
            .Where(x => string.IsNullOrWhiteSpace(context.SearchString) || x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase) || x.Value.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(100);
    }
}
