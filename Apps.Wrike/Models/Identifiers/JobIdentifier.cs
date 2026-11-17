namespace Apps.Wrike.Models.Identifiers;

public class JobIdentifier
{
    [Display("Job ID", Description = "Map the job ID returned by a batch submission. Jobs cannot be enumerated.")]
    public string JobId { get; set; } = string.Empty;
}
