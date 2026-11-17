namespace Apps.Wrike.Models.Responses;

public class FormItemResponse
{
    [Display("Item ID")]
    [JsonProperty("id")]
    public string ItemId { get; set; } = string.Empty;

    [Display("Title")]
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;
}
