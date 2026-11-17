namespace Apps.Wrike.Handlers;

public class FolderHandler(InvocationContext invocationContext, [ActionParameter] SpaceIdentifier parent) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var endpoint = string.IsNullOrWhiteSpace(parent.SpaceId)
            ? "folders"
            : $"spaces/{InputValidator.Id(parent.SpaceId, "Space ID")}/folders";
        var term = string.IsNullOrWhiteSpace(context.SearchString) ? null : context.SearchString.Trim();
        var items = await Client.Paginate<FolderResponse>(
            () => new RestRequest(endpoint).Optional("title", term),
            supportsPaging: term is not null,
            maximum: 10000,
            failOnLimit: true);

        return items.Select(x => new DataSourceItem(x.FolderId, $"{x.Title} [{x.FolderId}]"))
            .Take(100);
    }
}
