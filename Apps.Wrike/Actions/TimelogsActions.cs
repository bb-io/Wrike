namespace Apps.Wrike.Actions;

[ActionList("Timelogs")]
public class TimelogsActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Search timelogs", Description = "Search timelogs.")]
    public async Task<SearchTimelogResponse> SearchTimelogs([ActionParameter] TaskIdentifier input)
    {
        return new() { Timelogs = await Client.Paginate<TimelogResponse>(() => new RestRequest($"tasks/{InputValidator.Id(input.TaskId, "Task ID")}/timelogs"), true) };
    }

    [Action("Create timelog", Description = "Create timelog.")]
    public async Task<TimelogResponse> CreateTimelog([ActionParameter] TimelogCreateRequest input)
    {
        if (input.Hours < 0 || input.Hours > 24)
            throw new PluginMisconfigurationException("Hours must be between 0 and 24.");
        var result = await Client.ExecuteWithErrorHandling<Envelope<TimelogResponse>>(new RestRequest($"tasks/{InputValidator.Id(input.TaskId, "Task ID")}/timelogs", Method.Post).Optional("hours", input.Hours).Optional("trackedDate", input.TrackedDate).Optional("comment", input.Comment).Optional("categoryId", input.CategoryId));
        return result.Data.First();
    }

    [Action("Update timelog", Description = "Update timelog.")]
    public async Task<TimelogResponse> UpdateTimelog([ActionParameter] TimelogUpdateRequest input)
    {
        if (input.Hours < 0 || input.Hours > 24)
            throw new PluginMisconfigurationException("Hours must be between 0 and 24.");
        var result = await Client.ExecuteWithErrorHandling<Envelope<TimelogResponse>>(new RestRequest($"timelogs/{InputValidator.Id(input.TimelogId, "Timelog ID")}", Method.Put).Optional("hours", input.Hours).Optional("trackedDate", input.TrackedDate).Optional("comment", input.Comment).Optional("categoryId", input.CategoryId));
        return result.Data.First();
    }

    [Action("Delete timelog", Description = "Delete timelog.")]
    public async Task DeleteTimelog([ActionParameter] TimelogIdentifier input)
    {
        await Client.ExecuteWithErrorHandling(new RestRequest($"timelogs/{InputValidator.Id(input.TimelogId, "Timelog ID")}", Method.Delete));
    }
}
