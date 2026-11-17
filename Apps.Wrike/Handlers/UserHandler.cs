namespace Apps.Wrike.Handlers;

public class UserHandler(InvocationContext invocationContext) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var items = await Client.Paginate<ContactResponse>(() => new RestRequest("contacts").Optional("types", new[] { "Person" }).Optional("name", context.SearchString), false, 10000, true);
        return items.Select(x => new DataSourceItem(x.UserId, $"{(x.FirstName + " " + x.LastName + " · " + x.PrimaryEmail)} [{x.UserId}]"))
            .Where(x => string.IsNullOrWhiteSpace(context.SearchString) || x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase) || x.Value.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(100);
    }
}
