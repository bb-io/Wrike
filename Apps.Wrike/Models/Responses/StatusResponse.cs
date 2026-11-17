namespace Apps.Wrike.Models.Responses;

public class StatusResponse
{
    [Display("Custom status ID")]
    [JsonProperty("id")]
    public string CustomStatusId { get; set; } = string.Empty;

    [Display("Name")]
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [Display("Group")]
    [JsonProperty("group")]
    public string Group { get; set; } = string.Empty;

    [Display("Hidden")]
    [JsonProperty("hidden")]
    public bool Hidden { get; set; }
}
