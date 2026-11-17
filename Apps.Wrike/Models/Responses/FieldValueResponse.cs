namespace Apps.Wrike.Models.Responses;

public class FieldValueResponse
{
    [Display("Custom field ID")]
    [JsonProperty("id")]
    public string CustomFieldId { get; set; } = string.Empty;

    [Display("Value")]
    [JsonProperty("value")]
    public string? Value { get; set; }
}
