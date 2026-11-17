using System.Text.RegularExpressions;

namespace Apps.Wrike.Actions;

[ActionList("Requests")]
public class RequestActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Send request", Description = "Send an advanced request to any API v4 endpoint on the connected host.")]
    public async Task<JsonResponse> SendRequest([ActionParameter] SendRequestInput input)
    {
        var path = InputValidator.Required(input.Path, "Path").TrimStart('/');
        if (path.StartsWith("api/v4/", StringComparison.Ordinal))
            path = path[7..];

        if (!Regex.IsMatch(path, "^[A-Za-z0-9_,/-]+$") || path.Contains("//") || input.Path.StartsWith("//"))
            throw new PluginMisconfigurationException("Use a relative API v4 path without a host, query string, encoded characters or traversal. Supply parameters separately.");

        var method = input.Method.ToUpperInvariant() switch
        {
            "GET" => Method.Get,
            "POST" => Method.Post,
            "PUT" => Method.Put,
            "DELETE" => Method.Delete,
            _ => throw new PluginMisconfigurationException("Select GET, POST, PUT or DELETE.")
        };
        var request = new RestRequest(path, method);
        if (input.ParametersJson != null)
        {
            var parameters = InputValidator.Json<Dictionary<string, object?>>(input.ParametersJson, "Parameters JSON");
            if (parameters.Keys.Any(x => x.Equals("access_token", StringComparison.OrdinalIgnoreCase) || x.Equals("method", StringComparison.OrdinalIgnoreCase)))
                throw new PluginMisconfigurationException("Authentication and HTTP method are managed by the connection and Method input.");

            foreach (var parameter in parameters)
                request.Optional(parameter.Key, parameter.Value);
        }
        if (input.BodyJson != null)
        {
            InputValidator.Json<object>(input.BodyJson, "Body JSON");
            if (method == Method.Get)
                throw new PluginMisconfigurationException("GET requests cannot include Body JSON.");

            request.AddStringBody(input.BodyJson, ContentType.Json);
        }
        var response = await Client.ExecuteWithErrorHandling(request);
        return new() { ResponseJson = response.Content ?? string.Empty };
    }
}
