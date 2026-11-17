namespace Apps.Wrike.Models.Responses;

public class FieldSettingsResponse
{
    [Display("Read only")]
    [JsonProperty("readOnly")]
    public bool ReadOnly { get; set; }

    [Display("Values")]
    [JsonProperty("values")]
    public List<string> Values { get; set; } = [];

    [Display("Options")]
    [JsonProperty("options")]
    public List<FieldOptionResponse> Options { get; set; } = [];

    [Display("Allow other values")]
    [JsonProperty("allowOtherValues")]
    public bool AllowOtherValues { get; set; }
}
