namespace Apps.Wrike.Actions;

[ActionList("Administration")]
public class AdministrationActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Search groups", Description = "Search groups.")]
    public async Task<SearchGroupResponse> SearchGroups()
    {
        return new() { Groups = await Client.Paginate<GroupResponse>(() => new RestRequest("groups"), true, pageSize: 100, tokenParameter: "pageToken") };
    }

    [Action("Create group", Description = "Create group.")]
    public async Task<GroupResponse> CreateGroup([ActionParameter] GroupCreateRequest input)
    {
        InputValidator.Required(input.Title, "Title");
        var result = await Client.ExecuteWithErrorHandling<Envelope<GroupResponse>>(new RestRequest("groups", Method.Post).Optional("title", input.Title).Optional("members", input.MemberIds).Optional("parent", input.ParentGroupId));
        return result.Data.First();
    }

    [Action("Update group", Description = "Update group.")]
    public async Task<GroupResponse> UpdateGroup([ActionParameter] GroupUpdateRequest input)
    {
        var result = await Client.ExecuteWithErrorHandling<Envelope<GroupResponse>>(new RestRequest($"groups/{InputValidator.Id(input.GroupId, "Group ID")}", Method.Put).Optional("title", input.Title).Optional("addMembers", input.AddMemberIds).Optional("removeMembers", input.RemoveMemberIds));
        return result.Data.First();
    }

    [Action("Delete group", Description = "Delete group.")]
    public async Task DeleteGroup([ActionParameter] GroupIdentifier input)
    {
        await Client.ExecuteWithErrorHandling(new RestRequest($"groups/{InputValidator.Id(input.GroupId, "Group ID")}", Method.Delete));
    }

    [Action("Get user", Description = "Get user.")]
    public async Task<ContactResponse> GetUser([ActionParameter] UserIdentifier input)
    {
        var result = await Client.ExecuteWithErrorHandling<Envelope<ContactResponse>>(new RestRequest($"users/{InputValidator.Id(input.UserId, "User ID")}", Method.Get));
        return result.Data.First();
    }

    [Action("Update user active status", Description = "Update user active status.")]
    public async Task<ContactResponse> UpdateUserActiveStatus([ActionParameter] UserActiveRequest input)
    {
        var result = await Client.ExecuteWithErrorHandling<Envelope<ContactResponse>>(new RestRequest($"users/{InputValidator.Id(input.UserId, "User ID")}", Method.Put).Optional("active", input.Active));
        return result.Data.First();
    }
}
