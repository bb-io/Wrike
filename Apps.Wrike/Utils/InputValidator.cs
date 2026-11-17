namespace Apps.Wrike.Utils;

public static class InputValidator
{
    public static string Required(string? value, string label)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new PluginMisconfigurationException($"Please fill in '{label}'.");

        return value;
    }

    public static string Id(string? value, string label)
    {
        return Uri.EscapeDataString(Required(value, label));
    }

    public static string Host(string? value)
    {
        var host = Required(value, "Wrike host").Trim().ToLowerInvariant();
        if (host is not ("www.wrike.com" or "app-eu.wrike.com" or "app-us2.wrike.com"))
            throw new PluginMisconfigurationException("Select a supported Wrike host: www.wrike.com, app-eu.wrike.com or app-us2.wrike.com.");

        return host;
    }

    public static T Json<T>(string value, string label)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(Required(value, label))
                ?? throw new PluginMisconfigurationException($"Enter valid JSON in '{label}'.");
        }
        catch (JsonException)
        {
            throw new PluginMisconfigurationException($"Enter valid JSON in '{label}'.");
        }
    }

    public static int Maximum(int? value)
    {
        if (value is < 1 or > 10000)
            throw new PluginMisconfigurationException("Maximum results must be between 1 and 10000.");

        return value ?? 1000;
    }
}
