using Apps.Wrike.Api;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Wrike.Connections;

public class ConnectionValidator(InvocationContext invocationContext) : BaseInvocable(invocationContext), IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, CancellationToken cancellationToken)
    {
        try
        {
            using var client = new WrikeClient(authenticationCredentialsProviders);
            var response = await client.ExecuteWithErrorHandling<Envelope<ContactResponse>>(new RestRequest("contacts").Optional("me", true));
            var user = response.Data.First();
            return new() { IsValid = true, Message = $"Connected as {user.FirstName} {user.LastName}" };
        }
        catch (Exception ex)
        {
            return new() { IsValid = false, Message = ex is PluginApplicationException or PluginMisconfigurationException ? ex.Message : "Could not validate the Wrike connection. Reconnect and check the selected host." };
        }
    }
}
