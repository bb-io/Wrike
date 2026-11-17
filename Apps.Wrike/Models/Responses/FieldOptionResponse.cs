namespace Apps.Wrike.Models.Responses;

public class FieldOptionResponse
{
    [Display("Value")]
    [JsonProperty("value")]
    public string Value { get; set; } = string.Empty;

    [Display("Color")]
    [JsonProperty("color")]
    public string? Color { get; set; }
}
