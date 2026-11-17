using Apps.Wrike.Api;

namespace Apps.Wrike;

public class Invocable : BaseInvocable
{
    protected WrikeClient Client { get; }

    public Invocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new WrikeClient(invocationContext.AuthenticationCredentialsProviders);
    }
}
