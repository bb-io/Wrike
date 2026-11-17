namespace Apps.Wrike.Models.Responses;

public class SearchWorkflowResponse
{
    [Display("Workflows")]
    public List<WorkflowResponse> Workflows { get; set; } = [];
}
