namespace Apps.Wrike.Models.Responses;

public class JobResponse
{
    [Display("Job ID")]
    [JsonProperty("id")]
    public string JobId { get; set; } = string.Empty;

    [Display("Status")]
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [Display("Error message")]
    [JsonProperty("errorMessage")]
    public string? ErrorMessage { get; set; }

    [Display("Progress percent")]
    [JsonProperty("progressPercent")]
    public double? ProgressPercent { get; set; }
}
