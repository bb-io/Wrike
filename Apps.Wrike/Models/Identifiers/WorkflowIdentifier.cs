namespace Apps.Wrike.Models.Identifiers;

public class WorkflowIdentifier : SpaceIdentifier
{
    [Display("Workflow ID")]
    [DataSource(typeof(WorkflowHandler))]
    public string? WorkflowId { get; set; }
}
