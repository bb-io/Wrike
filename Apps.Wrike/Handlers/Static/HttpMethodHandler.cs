namespace Apps.Wrike.Handlers.Static;

public class HttpMethodHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new("GET", "GET"),
        new("POST", "POST"),
        new("PUT", "PUT"),
        new("DELETE", "DELETE")
    ];
}
