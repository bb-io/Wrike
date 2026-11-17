using Apps.Wrike.Constants;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;

namespace Apps.Wrike.Auth.OAuth2;

public class OAuth2AuthorizeService(InvocationContext invocationContext) : BaseInvocable(invocationContext), IOAuth2AuthorizeService
{
    public string GetAuthorizationUrl(Dictionary<string, string> values)
    {
        InputValidator.Host(values.GetValueOrDefault(CredsNames.Host));
        var scopes = "wsReadWrite";
        var profile = values.GetValueOrDefault(CredsNames.PermissionProfile);
        if (!string.IsNullOrWhiteSpace(profile) && profile is not "admin" and not "work")
            throw new PluginMisconfigurationException("Select Work management or Work management and administration in the connection permissions.");

        if (string.IsNullOrWhiteSpace(profile) || profile == "admin")
            scopes += ",amReadWriteUser,amReadWriteGroup";

        var parameters = new Dictionary<string, string>
        {
            ["client_id"] = InputValidator.Required(values.GetValueOrDefault(CredsNames.ClientId), "Client ID"),
            ["redirect_uri"] = InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString(),
            ["response_type"] = "code",
            ["scope"] = scopes,
            ["state"] = values["state"]
        };
        return "https://login.wrike.com/oauth2/authorize/v4?" + string.Join("&", parameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));
    }
}
