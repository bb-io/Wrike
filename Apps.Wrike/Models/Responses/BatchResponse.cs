namespace Apps.Wrike.Models.Responses;

public class BatchResponse : JobResponse
{
    [Display("Result JSON", Description = "Complete provider result, including per-operation statuses, errors and correlation IDs.")]
    public string? ResultJson { get; set; }
}
