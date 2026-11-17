namespace Apps.Wrike.Models.Responses;

public class UrlResponse
{
    [Display("URL")]
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
}
