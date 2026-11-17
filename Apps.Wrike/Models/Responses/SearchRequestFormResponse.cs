namespace Apps.Wrike.Models.Responses;

public class SearchRequestFormResponse
{
    [Display("Request forms")]
    public List<RequestFormResponse> RequestForms { get; set; } = [];
}
