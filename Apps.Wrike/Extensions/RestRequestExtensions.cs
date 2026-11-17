using System.Globalization;

namespace Apps.Wrike.Extensions;

public static class RestRequestExtensions
{
    public static RestRequest Optional(this RestRequest request, string name, object? value)
    {
        if (value is null)
            return request;

        var text = value switch
        {
            string s => s,
            bool b => b ? "true" : "false",
            DateTime d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            IFormattable n => n.ToString(null, CultureInfo.InvariantCulture),
            _ => JsonConvert.SerializeObject(value)
        };
        request.AddQueryParameter(name, text);
        return request;
    }
}
