namespace Apps.Wrike.Actions;

[ActionList("Discovery")]
public class DiscoveryActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Search spaces", Description = "Search spaces.")]
    public async Task<SearchSpaceResponse> SearchSpaces()
    {
        return new() { Spaces = await Client.Paginate<SpaceResponse>(() => new RestRequest("spaces"), false) };
    }

    [Action("Search folders", Description = "Find folders by name in a space, or across the accessible account.")]
    public async Task<SearchFolderResponse> SearchFolders([ActionParameter] FolderSearchRequest input)
    {
        var endpoint = string.IsNullOrWhiteSpace(input.SpaceId)
            ? "folders"
            : $"spaces/{InputValidator.Id(input.SpaceId, "Space ID")}/folders";
        var term = string.IsNullOrWhiteSpace(input.SearchTerm) ? null : input.SearchTerm.Trim();
        return new()
        {
            Folders = await Client.Paginate<FolderResponse>(
                () => new RestRequest(endpoint).Optional("title", term),
                supportsPaging: term is not null,
                failOnLimit: true)
        };
    }

    [Action("Search workflows", Description = "Search workflows.")]
    public async Task<SearchWorkflowResponse> SearchWorkflows([ActionParameter] SpaceIdentifier input)
    {
        return new() { Workflows = await Client.Paginate<WorkflowResponse>(() => new RestRequest($"spaces/{InputValidator.Id(input.SpaceId, "Space ID")}/workflows"), false) };
    }

    [Action("Search custom fields", Description = "Search custom fields.")]
    public async Task<SearchCustomFieldResponse> SearchCustomFields([ActionParameter] SpaceIdentifier input)
    {
        return new() { CustomFields = await Client.Paginate<CustomFieldResponse>(() => new RestRequest($"spaces/{InputValidator.Id(input.SpaceId, "Space ID")}/customfields"), false) };
    }

    [Action("Search users", Description = "Search users.")]
    public async Task<SearchContactResponse> SearchUsers()
    {
        return new() { Contacts = await Client.Paginate<ContactResponse>(() => new RestRequest("contacts").Optional("types", new[] { "Person" }), false) };
    }
}
