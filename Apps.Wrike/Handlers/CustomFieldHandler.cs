namespace Apps.Wrike.Handlers;

public class CustomFieldHandler(InvocationContext invocationContext, [ActionParameter] SpaceIdentifier parent) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(parent.SpaceId))
            return [];

        var items = await Client.Paginate<CustomFieldResponse>(() => new RestRequest($"spaces/{InputValidator.Id(parent.SpaceId, "Space ID")}/customfields"), false, 10000, true);
        return items.Select(x => new DataSourceItem(x.CustomFieldId, $"{(x.Title + " · " + x.Type)} [{x.CustomFieldId}]"))
            .Where(x => string.IsNullOrWhiteSpace(context.SearchString) || x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase) || x.Value.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(100);
    }
}
