using System.Net;
using System.Text.RegularExpressions;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Wrike.Webhooks;

[WebhookList("Tasks")]
public class TaskWebhooks(InvocationContext invocationContext) : BaseInvocable(invocationContext), IWebhookHandshakeHandler
{
    public Task<HttpResponseMessage?> HandleHandshakeAsync(WebhookRequest request)
    {
        var challenge = WebhookSecurity.Header(request, "X-Hook-Secret");
        if (string.IsNullOrEmpty(challenge))
            return Task.FromResult<HttpResponseMessage?>(null);

        var secret = WebhookSecurity.Secret(InvocationContext.AuthenticationCredentialsProviders);
        if (!Regex.IsMatch(challenge, "^[A-Za-z0-9]{1,100}$") || !WebhookSecurity.Verify(secret, request))
            return Task.FromResult<HttpResponseMessage?>(new HttpResponseMessage(HttpStatusCode.Unauthorized));

        var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Headers.Add("X-Hook-Secret", WebhookSecurity.Sign(secret, challenge));
        return Task.FromResult<HttpResponseMessage?>(response);
    }

    [Webhook("On task created", typeof(TaskCreatedHandler), Description = "Receive task-creation events in a selected space.")]
    [MultipleEvents]
    public Task<WebhookResponse<List<TaskEventResponse>>> OnTaskCreated(WebhookRequest request, [WebhookParameter(true)] EventInput input)
        => Handle(request, input, "TaskCreated");

    [Webhook("On task status changed", typeof(TaskStatusHandler), Description = "Receive task-status changes in a selected space.")]
    [MultipleEvents]
    public Task<WebhookResponse<List<TaskEventResponse>>> OnTaskStatusChanged(WebhookRequest request, [WebhookParameter(true)] EventInput input)
        => Handle(request, input, "TaskStatusChanged");

    [Webhook("On task custom field changed", typeof(TaskCustomFieldHandler), Description = "Receive custom-field changes in a selected space.")]
    [MultipleEvents]
    public Task<WebhookResponse<List<TaskEventResponse>>> OnTaskCustomFieldChanged(WebhookRequest request, [WebhookParameter(true)] EventInput input)
        => Handle(request, input, "TaskCustomFieldChanged");

    private Task<WebhookResponse<List<TaskEventResponse>>> Handle(WebhookRequest request, EventInput input, string topic)
    {
        var secret = WebhookSecurity.Secret(InvocationContext.AuthenticationCredentialsProviders);
        if (!WebhookSecurity.Verify(secret, request))
            throw new PluginMisconfigurationException("The webhook signature is invalid. Reconnect and recreate the webhook if the client secret changed.");

        var body = request.Body?.ToString() ?? string.Empty;
        var events = body.TrimStart().StartsWith('[')
            ? InputValidator.Json<List<TaskEventResponse>>(body, "Webhook body")
            : new List<TaskEventResponse> { InputValidator.Json<TaskEventResponse>(body, "Webhook body") };
        var matching = events.Where(x => x.EventType == topic && (topic != "TaskCustomFieldChanged" || string.IsNullOrWhiteSpace(input.CustomFieldId) || x.CustomFieldId == input.CustomFieldId)).ToList();
        if (matching.Count > 100)
            throw new PluginApplicationException("Wrike delivered more than 100 matching events in one notification. Narrow the webhook scope; no events were silently dropped.");

        var deliveryId = WebhookSecurity.Header(request, "Idempotency-Key");
        foreach (var item in matching)
            item.DeliveryId = string.IsNullOrEmpty(deliveryId) ? null : deliveryId;

        return Task.FromResult(new WebhookResponse<List<TaskEventResponse>>
        {
            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
            Result = matching.Count == 0 ? null : matching,
            ReceivedWebhookRequestType = matching.Count == 0 ? WebhookRequestType.Preflight : WebhookRequestType.Default
        });
    }
}
