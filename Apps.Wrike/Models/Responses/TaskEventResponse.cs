namespace Apps.Wrike.Models.Responses;

public class TaskEventResponse
{
    [Display("Task ID")]
    [JsonProperty("taskId")]
    public string TaskId { get; set; } = string.Empty;

    [Display("Webhook ID")]
    [JsonProperty("webhookId")]
    public string WebhookId { get; set; } = string.Empty;

    [Display("Event type")]
    [JsonProperty("eventType")]
    public string EventType { get; set; } = string.Empty;

    [Display("Event author ID")]
    [JsonProperty("eventAuthorId")]
    public string EventAuthorId { get; set; } = string.Empty;

    [Display("Last updated date")]
    [JsonProperty("lastUpdatedDate")]
    public DateTime? LastUpdatedDate { get; set; }

    [Display("Status")]
    [JsonProperty("status")]
    public string? Status { get; set; }

    [Display("Old status")]
    [JsonProperty("oldStatus")]
    public string? OldStatus { get; set; }

    [Display("Custom status ID")]
    [JsonProperty("customStatusId")]
    public string? CustomStatusId { get; set; }

    [Display("Old custom status ID")]
    [JsonProperty("oldCustomStatusId")]
    public string? OldCustomStatusId { get; set; }

    [Display("Custom field ID")]
    [JsonProperty("customFieldId")]
    public string? CustomFieldId { get; set; }

    [Display("Value")]
    [JsonProperty("value")]
    public string? Value { get; set; }

    [Display("Old value")]
    [JsonProperty("oldValue")]
    public string? OldValue { get; set; }

    [Display("Delivery ID")]
    public string? DeliveryId { get; set; }
}
