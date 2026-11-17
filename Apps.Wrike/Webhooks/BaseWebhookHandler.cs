using Apps.Wrike.Api;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Wrike.Webhooks;

public abstract class BaseWebhookHandler(InvocationContext invocationContext, EventInput input) : BaseInvocable(invocationContext), IWebhookEventHandler, IAsyncValidatableWebhookEventHandler
{
    protected abstract string Topic { get; }

    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> credentials, Dictionary<string, string> values)
    {
        using var client = new WrikeClient(credentials);
        var payloadUrl = InputValidator.Required(values.GetValueOrDefault("payloadUrl"), "Callback URL");
        var hooks = await client.Paginate<WebhookDto>(() => new RestRequest("webhooks"));
        var existing = hooks.FirstOrDefault(x => x.HookUrl == payloadUrl);
        if (existing != null)
        {
            if (existing.Status == "Active")
                return;

            throw new PluginMisconfigurationException("This callback already has a suspended webhook. Deactivate and reactivate the Bird to recreate it.");
        }
        var request = new RestRequest($"spaces/{InputValidator.Id(input.SpaceId, "Space ID")}/webhooks", Method.Post)
            .Optional("hookUrl", payloadUrl).Optional("recursive", true)
            .Optional("secret", WebhookSecurity.Secret(credentials));
        if (Topic == "TaskCustomFieldChanged" && !string.IsNullOrWhiteSpace(input.CustomFieldId))
            request.Optional("parameterisedEvents", new[] { new { @event = Topic, customFields = new[] { input.CustomFieldId } } });
        else
            request.Optional("events", new[] { Topic });

        await client.ExecuteWithErrorHandling(request);
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> credentials, Dictionary<string, string> values)
    {
        using var client = new WrikeClient(credentials);
        var hooks = await client.Paginate<WebhookDto>(() => new RestRequest("webhooks"));
        foreach (var hook in hooks.Where(x => x.HookUrl == values["payloadUrl"]))
            await client.ExecuteWithErrorHandling(new RestRequest($"webhooks/{InputValidator.Id(hook.WebhookId, "Webhook ID")}", Method.Delete));
    }

    public async Task<WebhookSubscriptionValidationResponse> ValidateSubscription(IEnumerable<AuthenticationCredentialsProvider> credentials, Dictionary<string, string> values)
    {
        try
        {
            using var client = new WrikeClient(credentials);
            var hooks = await client.Paginate<WebhookDto>(() => new RestRequest("webhooks"));
            return hooks.Any(x => x.HookUrl == values["payloadUrl"] && x.Status == "Active")
                ? new() { IsValid = true }
                : new() { IsValid = false, Message = "The Wrike webhook is missing or suspended. Check callback accessibility, then deactivate and reactivate the Bird." };
        }
        catch (PluginMisconfigurationException ex)
        {
            return new() { IsValid = false, Message = ex.Message };
        }
    }
}
