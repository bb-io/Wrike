namespace Apps.Wrike.Handlers;

public class TimelogHandler(InvocationContext invocationContext, [ActionParameter] TaskIdentifier parent) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(parent.TaskId))
            return [];

        var items = await Client.Paginate<TimelogResponse>(() => new RestRequest($"tasks/{InputValidator.Id(parent.TaskId, "Task ID")}/timelogs"), true, 10000, true);
        return items.Select(x => new DataSourceItem(x.TimelogId, $"{(x.TrackedDate.ToString("yyyy-MM-dd") + " · " + x.Hours + "h · " + x.Comment)} [{x.TimelogId}]"))
            .Where(x => string.IsNullOrWhiteSpace(context.SearchString) || x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase) || x.Value.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(100);
    }
}
