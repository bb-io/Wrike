namespace Apps.Wrike.Models.Responses;

public class RequestFormResponse
{
    [Display("Request form ID")]
    [JsonProperty("id")]
    public string RequestFormId { get; set; } = string.Empty;

    [Display("Title")]
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [Display("Description")]
    [JsonProperty("description")]
    public string? Description { get; set; }

    [Display("Pages")]
    [JsonProperty("pages")]
    public List<FormPageResponse> Pages { get; set; } = [];
}
