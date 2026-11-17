namespace Apps.Wrike.Handlers;

public class FieldOptionHandler(InvocationContext invocationContext, [ActionParameter] CustomFieldIdentifier parent) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(parent.CustomFieldId))
            return [];

        var fields = await Client.ExecuteWithErrorHandling<Envelope<CustomFieldResponse>>(new RestRequest($"customfields/{InputValidator.Id(parent.CustomFieldId, "Custom field ID")}"));
        var field = fields.Data.First();
        if (field.Type != "DropDown" || field.Settings == null)
            return [];

        return field.Settings.Values.Concat(field.Settings.Options.Select(x => x.Value)).Distinct()
            .Where(x => string.IsNullOrWhiteSpace(context.SearchString) || x.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x, x));
    }
}
