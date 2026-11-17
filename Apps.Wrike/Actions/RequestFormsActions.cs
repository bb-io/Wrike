namespace Apps.Wrike.Actions;

[ActionList("RequestForms")]
public class RequestFormsActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Search request forms", Description = "Search request forms.")]
    public async Task<SearchRequestFormResponse> SearchRequestForms([ActionParameter] SpaceIdentifier input)
    {
        return new() { RequestForms = await Client.Paginate<RequestFormResponse>(() => new RestRequest($"spaces/{InputValidator.Id(input.SpaceId, "Space ID")}/request_forms"), true, pageSize: 100) };
    }

    [Action("Get request form", Description = "Get request form.")]
    public async Task<RequestFormResponse> GetRequestForm([ActionParameter] RequestFormIdentifier input)
    {
        var result = await Client.ExecuteWithErrorHandling<Envelope<RequestFormResponse>>(new RestRequest($"request_forms/{InputValidator.Id(input.RequestFormId, "Request form ID")}", Method.Get));
        return result.Data.First();
    }

    [Action("Submit request form", Description = "Submit request form.")]
    public async Task<FormSubmitResponse> SubmitRequestForm([ActionParameter] FormSubmitRequest input)
    {
        var result = await Client.ExecuteWithErrorHandling<Envelope<FormSubmitResponse>>(new RestRequest($"request_forms/{InputValidator.Id(input.RequestFormId, "Request form ID")}/submit", Method.Post).Optional("formFields", InputValidator.Json<List<Dictionary<string, object>>>(input.AnswersJson, "Answers JSON")));
        return result.Data.First();
    }

    [Action("Create prefilled form URL", Description = "Create prefilled form URL.")]
    public async Task<UrlResponse> CreatePrefilledFormUrl([ActionParameter] FormSubmitRequest input)
    {
        var result = await Client.ExecuteWithErrorHandling<Envelope<UrlResponse>>(new RestRequest($"request_forms/{InputValidator.Id(input.RequestFormId, "Request form ID")}/prefill_url", Method.Post).Optional("formFields", InputValidator.Json<List<Dictionary<string, object>>>(input.AnswersJson, "Answers JSON")));
        return result.Data.First();
    }
}
