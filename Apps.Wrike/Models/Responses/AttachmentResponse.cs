namespace Apps.Wrike.Models.Responses;

public class AttachmentResponse
{
    [Display("Attachment ID")]
    [JsonProperty("id")]
    public string AttachmentId { get; set; } = string.Empty;

    [Display("Name")]
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [Display("Content type")]
    [JsonProperty("contentType")]
    public string? ContentType { get; set; }

    [Display("Size")]
    [JsonProperty("size")]
    public long Size { get; set; }

    [Display("Type")]
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [Display("URL")]
    [JsonProperty("url")]
    public string? Url { get; set; }
}
