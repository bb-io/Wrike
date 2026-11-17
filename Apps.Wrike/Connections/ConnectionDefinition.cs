using Apps.Wrike.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Wrike.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups =>
    [
        new()
        {
            Name = "OAuth 2.0",
            AuthenticationType = ConnectionAuthenticationType.OAuth2,
            ConnectionProperties =
            [
                new(CredsNames.ClientId) { DisplayName = "Client ID" },
                new(CredsNames.ClientSecret) { DisplayName = "Client Secret", Sensitive = true },
                new(CredsNames.Host)
                {
                    DisplayName = "Wrike host",
                    Description = "Must match the data center returned by OAuth.",
                    DataItems = [new("www.wrike.com", "US — www.wrike.com"), new("app-eu.wrike.com", "EU — app-eu.wrike.com"), new("app-us2.wrike.com", "US2 — app-us2.wrike.com")]
                },
                new(CredsNames.PermissionProfile)
                {
                    DisplayName = "Permissions",
                    Description = "Default: Work management and administration. Includes user/group scopes; Wrike account rights and plan limits still apply. Select Work management for narrower access.",
                    DataItems = [new("admin", "Work management and administration (default)"), new("work", "Work management")]
                }
            ]
        }
    ];

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(Dictionary<string, string> values)
        => values.Select(x => new AuthenticationCredentialsProvider(x.Key, x.Value));
}
