using System.Security.Cryptography;
using System.Text;
using Apps.Wrike.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Wrike.Webhooks;

public static class WebhookSecurity
{
    public static string Secret(IEnumerable<AuthenticationCredentialsProvider> credentials)
    {
        var clientSecret = InputValidator.Required(credentials.FirstOrDefault(x => x.KeyName == CredsNames.ClientSecret)?.Value, "Client Secret");
        return Convert.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(clientSecret), Encoding.UTF8.GetBytes("Blackbird.Wrike.webhook.v1"))).ToLowerInvariant();
    }

    public static string Header(WebhookRequest request, string name)
        => request.Headers?.FirstOrDefault(x => x.Key.Equals(name, StringComparison.OrdinalIgnoreCase)).Value ?? string.Empty;

    public static string Sign(string secret, string value)
        => Convert.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    public static bool Verify(string secret, WebhookRequest request)
    {
        var signature = Header(request, "X-Hook-Signature");
        var body = request.Body?.ToString() ?? string.Empty;
        if (signature.Length != 64 || !signature.All(Uri.IsHexDigit))
            return false;

        return CryptographicOperations.FixedTimeEquals(Convert.FromHexString(signature), Convert.FromHexString(Sign(secret, body)));
    }
}
