using System.Globalization;
using Apps.Wrike.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;

namespace Apps.Wrike.Auth.OAuth2;

public class OAuth2TokenService(InvocationContext invocationContext) : BaseInvocable(invocationContext), IOAuth2TokenService, ITokenRefreshable
{
    public bool IsRefreshToken(Dictionary<string, string> values)
        => !DateTimeOffset.TryParse(values.GetValueOrDefault(CredsNames.ExpiresAt), CultureInfo.InvariantCulture, DateTimeStyles.None, out var expiry) || expiry <= DateTimeOffset.UtcNow.AddMinutes(2);

    public int? GetRefreshTokenExprireInMinutes(Dictionary<string, string> values)
        => DateTimeOffset.TryParse(values.GetValueOrDefault(CredsNames.ExpiresAt), CultureInfo.InvariantCulture, DateTimeStyles.None, out var expiry)
            ? Math.Max(0, (int)(expiry - DateTimeOffset.UtcNow).TotalMinutes - 2) : 0;

    public Task<Dictionary<string, string>> RequestToken(string state, string code, Dictionary<string, string> values, CancellationToken cancellationToken)
        => Exchange(values, new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString()
        }, "login.wrike.com", cancellationToken);

    public Task<Dictionary<string, string>> RefreshToken(Dictionary<string, string> values, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(values.GetValueOrDefault(CredsNames.RefreshToken)))
            throw new PluginMisconfigurationException("This Wrike connection has no saved OAuth refresh token. Create a new OAuth connection and select it in the Bird. Do not enter a refresh token manually.");

        return Exchange(values, new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = InputValidator.Required(values.GetValueOrDefault(CredsNames.RefreshToken), "Refresh token")
        }, InputValidator.Host(values.GetValueOrDefault(CredsNames.Host)), cancellationToken);
    }

    public Task RevokeToken(Dictionary<string, string> values)
        => Task.CompletedTask;

    private static async Task<Dictionary<string, string>> Exchange(Dictionary<string, string> values, Dictionary<string, string> parameters, string host, CancellationToken cancellationToken)
    {
        parameters["client_id"] = InputValidator.Required(values.GetValueOrDefault(CredsNames.ClientId), "Client ID");
        parameters["client_secret"] = InputValidator.Required(values.GetValueOrDefault(CredsNames.ClientSecret), "Client Secret");
        try
        {
            using var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });
            using var content = new FormUrlEncodedContent(parameters);
            using var response = await client.PostAsync($"https://{host}/oauth2/token", content, cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw new PluginMisconfigurationException($"Wrike authorization failed ({(int)response.StatusCode}). Check the client credentials and callback URL, then reconnect. A refresh is not automatically retried.");

            var token = JsonConvert.DeserializeObject<TokenDto>(await response.Content.ReadAsStringAsync(cancellationToken))!;
            if (token is null || string.IsNullOrWhiteSpace(token.AccessToken) || string.IsNullOrWhiteSpace(token.RefreshToken) || token.ExpiresIn <= 0)
                throw new PluginApplicationException("Wrike returned incomplete OAuth credentials. Create a new OAuth connection. No incomplete token set was saved.");

            var actualHost = InputValidator.Host(token.Host);
            if (actualHost != InputValidator.Host(values.GetValueOrDefault(CredsNames.Host)))
                throw new PluginMisconfigurationException($"The authorized account uses {actualHost}. Select this Wrike host and connect again.");

            var result = new Dictionary<string, string>(values)
            {
                [CredsNames.AccessToken] = token.AccessToken,
                [CredsNames.RefreshToken] = token.RefreshToken,
                [CredsNames.Host] = actualHost,
                [CredsNames.ExpiresAt] = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn).ToString("O")
            };
            return result;
        }
        catch (HttpRequestException)
        {
            throw new PluginApplicationException("Could not reach Wrike authorization. Reconnect if a token refresh was interrupted.");
        }
        catch (TaskCanceledException)
        {
            throw new PluginApplicationException("Wrike authorization timed out. Reconnect if a token refresh was interrupted.");
        }
        catch (JsonException)
        {
            throw new PluginApplicationException("Wrike returned an unreadable authorization response. Reconnect the connection.");
        }
    }
}
