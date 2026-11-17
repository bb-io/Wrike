namespace Apps.Wrike.Models.Responses;

public class TimelogResponse
{
    [Display("Timelog ID")]
    [JsonProperty("id")]
    public string TimelogId { get; set; } = string.Empty;

    [Display("Task ID")]
    [JsonProperty("taskId")]
    public string TaskId { get; set; } = string.Empty;

    [Display("User ID")]
    [JsonProperty("userId")]
    public string UserId { get; set; } = string.Empty;

    [Display("Hours")]
    [JsonProperty("hours")]
    public double Hours { get; set; }

    [Display("Tracked date")]
    [JsonProperty("trackedDate")]
    public DateTime TrackedDate { get; set; }

    [Display("Comment")]
    [JsonProperty("comment")]
    public string Comment { get; set; } = string.Empty;
}
