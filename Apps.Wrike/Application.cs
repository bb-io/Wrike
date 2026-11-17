using Apps.Wrike.Auth.OAuth2;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.Wrike;

public class Application(InvocationContext invocationContext) : BaseInvocable(invocationContext), IApplication, ICategoryProvider
{
    public string Name => "Wrike";

    public IEnumerable<ApplicationCategory> Categories
    {
        get => [ApplicationCategory.ProjectManagementAndProductivity, ApplicationCategory.TaskManagement];
        set { }
    }

    public T GetInstance<T>()
    {
        if (typeof(T) == typeof(IOAuth2AuthorizeService))
            return (T)(object)new OAuth2AuthorizeService(InvocationContext);

        if (typeof(T) == typeof(IOAuth2TokenService))
            return (T)(object)new OAuth2TokenService(InvocationContext);

        throw new InvalidOperationException($"Unsupported service: {typeof(T).Name}");
    }
}
