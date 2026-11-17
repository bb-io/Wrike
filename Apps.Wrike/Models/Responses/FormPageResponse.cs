namespace Apps.Wrike.Models.Responses;

public class FormPageResponse
{
    [Display("Page ID")]
    [JsonProperty("id")]
    public string PageId { get; set; } = string.Empty;

    [Display("Title")]
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [Display("Fields")]
    [JsonProperty("fields")]
    public List<FormFieldResponse> Fields { get; set; } = [];
}
