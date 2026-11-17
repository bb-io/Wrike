namespace Apps.Wrike.Models.Requests;

public class SendRequestInput
{
    [Display("Method")]
    [StaticDataSource(typeof(HttpMethodHandler))]
    public string Method { get; set; } = string.Empty;

    [Display("Path", Description = "Relative API v4 path only, such as tasks. No URL, query string or traversal.")]
    public string Path { get; set; } = string.Empty;

    [Display("Parameters JSON", Description = "JSON object of endpoint parameters. Structured values are JSON encoded.")]
    public string? ParametersJson { get; set; }

    [Display("Body JSON", Description = "Optional raw JSON body for endpoints that document one.")]
    public string? BodyJson { get; set; }
}
