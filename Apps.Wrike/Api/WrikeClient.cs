using System.Net;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Apps.Wrike.Constants;
using Polly;

namespace Apps.Wrike.Api;

public class WrikeClient : BlackBirdRestClient
{
    private readonly string[] secrets;

    public WrikeClient(IEnumerable<AuthenticationCredentialsProvider> credentials)
        : base(new RestClientOptions($"https://{InputValidator.Host(credentials.FirstOrDefault(x => x.KeyName == CredsNames.Host)?.Value)}/api/v4/")
        {
            FollowRedirects = false,
            Timeout = TimeSpan.FromSeconds(120)
        })
    {
        var token = InputValidator.Required(credentials.FirstOrDefault(x => x.KeyName == CredsNames.AccessToken)?.Value, "Access token");
        secrets = credentials.Where(x => x.KeyName.Contains("token") || x.KeyName.Contains("secret"))
            .Select(x => x.Value).Where(x => !string.IsNullOrEmpty(x)).ToArray();
        this.AddDefaultHeader("Authorization", $"bearer {token}");
    }

    public override async Task<RestResponse> ExecuteWithErrorHandling(RestRequest request)
    {
        try
        {
            var policy = Policy.HandleResult<RestResponse>(r => request.Method == Method.Get && r.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(3, (attempt, result, _) => RetryDelay(attempt, result.Result), (_, _, _, _) => Task.CompletedTask);
            var response = await policy.ExecuteAsync(() => this.ExecuteAsync(request));
            if (!response.IsSuccessful)
                throw ConfigureErrorException(response);

            return response;
        }
        catch (HttpRequestException)
        {
            throw new PluginApplicationException("Could not reach Wrike. Check service availability. For a write request, check whether the operation completed before retrying.");
        }
        catch (TaskCanceledException)
        {
            throw new PluginApplicationException("Wrike did not respond in time. Check whether a write completed before retrying.");
        }
    }

    public override async Task<T> ExecuteWithErrorHandling<T>(RestRequest request)
    {
        var response = await ExecuteWithErrorHandling(request);
        try
        {
            return JsonConvert.DeserializeObject<T>(response.Content!)
                ?? throw new PluginApplicationException("Wrike returned an empty response where data was expected.");
        }
        catch (JsonException)
        {
            throw new PluginApplicationException("Wrike returned an unreadable response. Check service availability before retrying.");
        }
    }

    public async Task<List<T>> Paginate<T>(Func<RestRequest> createRequest, bool supportsPaging = false, int maximum = 10000, bool failOnLimit = false, int pageSize = 1000, string tokenParameter = "nextPageToken")
    {
        var items = new List<T>();
        string? token = null;
        do
        {
            var request = createRequest();
            if (supportsPaging)
                request.Optional("pageSize", Math.Min(pageSize, maximum)).Optional(tokenParameter, token);

            var page = await ExecuteWithErrorHandling<Envelope<T>>(request);
            items.AddRange(page.Data);
            token = page.NextPageToken;
            if (items.Count >= maximum)
            {
                if (failOnLimit && (!string.IsNullOrEmpty(token) || items.Count > maximum))
                    throw new PluginMisconfigurationException("This search exceeds 10,000 results. Select a narrower parent or refine the search term; results have not been returned as a complete list.");

                return items.Take(maximum).ToList();
            }
        }
        while (!string.IsNullOrEmpty(token));

        return items;
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        if ((int)response.StatusCode == 0)
            return new PluginApplicationException("Could not reach Wrike. Check the connection host and service availability. Check whether a write completed before retrying.");

        ErrorDto? error = null;
        try
        {
            error = JsonConvert.DeserializeObject<ErrorDto>(response.Content ?? "{}");
        }
        catch (JsonException)
        {
        }

        var message = error?.Description ?? error?.Error ?? "The request could not be completed.";
        foreach (var secret in secrets)
            message = message.Replace(secret, "[redacted]", StringComparison.Ordinal);

        message = message.Length > 300 ? message[..300] : message;
        var status = (int)response.StatusCode;
        return status is >= 400 and < 500 && status != 429
            ? new PluginMisconfigurationException($"Wrike ({status}): {message} Check the inputs, connection permissions and resource access.")
            : new PluginApplicationException($"Wrike ({status}): {message} Check service availability; writes were not automatically retried.");
    }

    private static TimeSpan RetryDelay(int attempt, RestResponse response)
    {
        var header = response.Headers?.FirstOrDefault(x => string.Equals(x.Name, "Retry-After", StringComparison.OrdinalIgnoreCase))?.Value?.ToString();
        var delay = double.TryParse(header, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var seconds)
            ? TimeSpan.FromSeconds(Math.Max(0, seconds))
            : DateTimeOffset.TryParse(header, out var date) ? date - DateTimeOffset.UtcNow
            : TimeSpan.FromSeconds(Math.Pow(2, attempt) + Random.Shared.NextDouble());
        if (delay > TimeSpan.FromSeconds(60))
            throw new PluginApplicationException($"Wrike requested a longer rate-limit pause ({header}). Run the action after that pause.");

        return delay < TimeSpan.Zero ? TimeSpan.Zero : delay;
    }
}
