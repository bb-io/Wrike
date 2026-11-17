namespace Apps.Wrike.Models.Dtos;

public class WebhookDto
{
    [Display("Webhook ID")]
    [JsonProperty("id")]
    public string WebhookId { get; set; } = string.Empty;

    [Display("Hook URL")]
    [JsonProperty("hookUrl")]
    public string HookUrl { get; set; } = string.Empty;

    [Display("Status")]
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;
}
