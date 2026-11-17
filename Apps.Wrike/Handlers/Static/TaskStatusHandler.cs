namespace Apps.Wrike.Handlers.Static;

public class TaskStatusHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new("Active", "Active"),
        new("Completed", "Completed"),
        new("Deferred", "Deferred"),
        new("Cancelled", "Cancelled")
    ];
}
