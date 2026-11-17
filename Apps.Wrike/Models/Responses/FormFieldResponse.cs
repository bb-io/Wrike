namespace Apps.Wrike.Models.Responses;

public class FormFieldResponse
{
    [Display("Field ID")]
    [JsonProperty("id")]
    public string FieldId { get; set; } = string.Empty;

    [Display("Title")]
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [Display("Type")]
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [Display("Mandatory")]
    [JsonProperty("mandatory")]
    public bool Mandatory { get; set; }

    [Display("Helper text")]
    [JsonProperty("helperText")]
    public string? HelperText { get; set; }

    [Display("Items")]
    [JsonProperty("items")]
    public List<FormItemResponse> Items { get; set; } = [];
}
