namespace Apps.Wrike.Actions;

[ActionList("Approvals")]
public class ApprovalsActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Get approval", Description = "Get an approval and its current decisions.")]
    public async Task<ApprovalResponse> GetApproval([ActionParameter] ApprovalIdentifier input)
    {
        var result = await Client.ExecuteWithErrorHandling<Envelope<ApprovalResponse>>(new RestRequest($"approvals/{InputValidator.Id(input.ApprovalId, "Approval ID")}"));
        return result.Data.First();
    }

    [Action("Search approvals", Description = "Search approvals.")]
    public async Task<SearchApprovalResponse> SearchApprovals([ActionParameter] TaskIdentifier input)
    {
        return new() { Approvals = await Client.Paginate<ApprovalResponse>(() => new RestRequest($"tasks/{InputValidator.Id(input.TaskId, "Task ID")}/approvals"), false) };
    }

    [Action("Create approval", Description = "Create approval.")]
    public async Task<ApprovalResponse> CreateApproval([ActionParameter] ApprovalCreateRequest input)
    {
        var result = await Client.ExecuteWithErrorHandling<Envelope<ApprovalResponse>>(new RestRequest($"tasks/{InputValidator.Id(input.TaskId, "Task ID")}/approvals", Method.Post).Optional("description", input.Description).Optional("dueDate", input.DueDate).Optional("approvers", input.ApproverIds).Optional("attachments", input.AttachmentIds).Optional("autoFinishOnApprove", input.AutoFinishOnApprove).Optional("autoFinishOnReject", input.AutoFinishOnReject));
        return result.Data.First();
    }

    [Action("Update approval", Description = "Update approval.")]
    public async Task<ApprovalResponse> UpdateApproval([ActionParameter] ApprovalUpdateRequest input)
    {
        var result = await Client.ExecuteWithErrorHandling<Envelope<ApprovalResponse>>(new RestRequest($"approvals/{InputValidator.Id(input.ApprovalId, "Approval ID")}", Method.Put).Optional("description", input.Description).Optional("dueDate", input.DueDate).Optional("addApprovers", input.AddApproverIds).Optional("removeApprovers", input.RemoveApproverIds).Optional("addAttachments", input.AddAttachmentIds).Optional("removeAttachments", input.RemoveAttachmentIds));
        return result.Data.First();
    }

    [Action("Cancel approval", Description = "Cancel approval.")]
    public async Task CancelApproval([ActionParameter] ApprovalIdentifier input)
    {
        await Client.ExecuteWithErrorHandling(new RestRequest($"approvals/{InputValidator.Id(input.ApprovalId, "Approval ID")}", Method.Delete));
    }
}
