namespace Apps.Wrike.Handlers;

public class TaskHandler(InvocationContext invocationContext, [ActionParameter] TaskSearchRequest parent) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var path = !string.IsNullOrWhiteSpace(parent.FolderId)
            ? $"folders/{InputValidator.Id(parent.FolderId, "Folder ID")}/tasks"
            : !string.IsNullOrWhiteSpace(parent.SpaceId)
                ? $"spaces/{InputValidator.Id(parent.SpaceId, "Space ID")}/tasks"
                : "tasks";
        var term = string.IsNullOrWhiteSpace(context.SearchString) ? null : context.SearchString.Trim();
        var items = await Client.Paginate<TaskResponse>(
            () => new RestRequest(path)
                .Optional("descendants", path == "tasks" ? null : true)
                .Optional("subTasks", true)
                .Optional("title", term),
            supportsPaging: true,
            maximum: 10000,
            failOnLimit: true);

        return items.Select(x => new DataSourceItem(x.TaskId, $"{x.Title} [{x.TaskId}]"))
            .Take(100);
    }
}
