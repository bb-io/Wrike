namespace Apps.Wrike.Models.Responses;

public class TaskDatesResponse
{
    [Display("Type")]
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [Display("Start")]
    [JsonProperty("start")]
    public DateTime? Start { get; set; }

    [Display("Due")]
    [JsonProperty("due")]
    public DateTime? Due { get; set; }

    [Display("Duration")]
    [JsonProperty("duration")]
    public int? Duration { get; set; }
}
