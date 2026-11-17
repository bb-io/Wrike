namespace Apps.Wrike.Models.Responses;

public class CustomFieldResponse
{
    [Display("Custom field ID")]
    [JsonProperty("id")]
    public string CustomFieldId { get; set; } = string.Empty;

    [Display("Title")]
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [Display("Type")]
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;
    [Display("Settings")]
    [JsonProperty("settings")]
    public FieldSettingsResponse? Settings { get; set; }
}
