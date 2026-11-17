using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Wrike.Webhooks;

public class TaskCreatedHandler(InvocationContext invocationContext, [WebhookParameter(true)] EventInput input) : BaseWebhookHandler(invocationContext, input)
{
    protected override string Topic => "TaskCreated";
}
