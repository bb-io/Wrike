namespace Apps.Wrike.Models.Responses;

public class MetadataResponse
{
    [Display("Key")]
    [JsonProperty("key")]
    public string Key { get; set; } = string.Empty;

    [Display("Value")]
    [JsonProperty("value")]
    public string? Value { get; set; }
}
