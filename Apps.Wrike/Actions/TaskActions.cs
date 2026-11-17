namespace Apps.Wrike.Actions;

[ActionList("Tasks")]
public class TaskActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Search tasks", Description = "Search tasks in an accessible space or folder.")]
    public async Task<SearchTaskResponse> SearchTasks([ActionParameter] TaskSearchRequest input)
    {
        var path = !string.IsNullOrWhiteSpace(input.FolderId) ? $"folders/{InputValidator.Id(input.FolderId, "Folder ID")}/tasks"
            : !string.IsNullOrWhiteSpace(input.SpaceId) ? $"spaces/{InputValidator.Id(input.SpaceId, "Space ID")}/tasks" : "tasks";
        return new()
        {
            Tasks = await Client.Paginate<TaskResponse>(() => new RestRequest(path)
            .Optional("title", input.Title).Optional("subTasks", input.IncludeSubtasks ?? true)
            .Optional("descendants", path == "tasks" ? null : true)
            .Optional("fields", new[] { "description", "customFields", "metadata", "responsibleIds", "parentIds", "superTaskIds", "subTaskIds" }), true, InputValidator.Maximum(input.MaximumResults))
        };
    }

    [Action("Get task", Description = "Get a task and its current metadata.")]
    public async Task<TaskResponse> GetTask([ActionParameter] TaskIdentifier input)
    {
        var result = await Client.ExecuteWithErrorHandling<Envelope<TaskResponse>>(new RestRequest($"tasks/{InputValidator.Id(input.TaskId, "Task ID")}"));
        return result.Data.First();
    }

    [Action("Create task", Description = "Create a task in a selected folder or project.")]
    public async Task<TaskResponse> CreateTask([ActionParameter] CreateTaskRequest input)
    {
        var request = CreateRequest(input);
        var result = await Client.ExecuteWithErrorHandling<Envelope<TaskResponse>>(request);
        return result.Data.First();
    }

    [Action("Create subtask", Description = "Create a new task under a selected parent task.")]
    public async Task<TaskResponse> CreateSubtask([ActionParameter] CreateSubtaskRequest input)
    {
        InputValidator.Required(input.ParentTaskId, "Parent task ID");
        var result = await Client.ExecuteWithErrorHandling<Envelope<TaskResponse>>(CreateRequest(input).Optional("superTasks", new[] { input.ParentTaskId }));
        return result.Data.First();
    }

    [Action("Update task", Description = "Change supplied task fields while preserving omitted values.")]
    public async Task<TaskResponse> UpdateTask([ActionParameter] UpdateTaskRequest input)
    {
        ValidateStatus(input.Status, input.CustomStatusId);
        var request = new RestRequest($"tasks/{InputValidator.Id(input.TaskId, "Task ID")}", Method.Put)
            .Optional("title", input.Title).Optional("description", input.Description)
            .Optional("status", input.Status).Optional("customStatus", input.CustomStatusId)
            .Optional("addResponsibles", input.AddResponsibleIds).Optional("removeResponsibles", input.RemoveResponsibleIds);
        if (input.StartDate.HasValue || input.DueDate.HasValue)
        {
            var current = await GetTask(new TaskIdentifier { TaskId = input.TaskId });
            request.Optional("dates", Dates(input.StartDate ?? current.Dates?.Start, input.DueDate ?? current.Dates?.Due));
        }
        if (input.MetadataJson != null)
            request.Optional("metadata", InputValidator.Json<List<Dictionary<string, string?>>>(input.MetadataJson, "Metadata JSON"));

        var result = await Client.ExecuteWithErrorHandling<Envelope<TaskResponse>>(request);
        return result.Data.First();
    }

    [Action("Delete task", Description = "Delete the selected task.")]
    public async Task DeleteTask([ActionParameter] TaskIdentifier input)
        => await Client.ExecuteWithErrorHandling(new RestRequest($"tasks/{InputValidator.Id(input.TaskId, "Task ID")}", Method.Delete));

    [Action("Update task custom field", Description = "Set one custom field value without replacing unrelated fields.")]
    public async Task<TaskResponse> UpdateCustomField([ActionParameter] FieldValueRequest input)
    {
        InputValidator.Required(input.CustomFieldId, "Custom field ID");
        var result = await Client.ExecuteWithErrorHandling<Envelope<TaskResponse>>(new RestRequest($"tasks/{InputValidator.Id(input.TaskId, "Task ID")}", Method.Put)
            .Optional("customFields", new[] { new { id = input.CustomFieldId, value = input.Value } }));
        return result.Data.First();
    }

    [Action("Update tasks in bulk", Description = "Apply a shared status, assignment or custom-field change to up to 100 tasks.")]
    public async Task<SearchTaskResponse> UpdateTasksInBulk([ActionParameter] BulkTaskRequest input)
    {
        if (input.TaskIds.Count is < 1 or > 100)
            throw new PluginMisconfigurationException("Provide between 1 and 100 task IDs.");

        if (input.CustomStatusId == null && input.AddResponsibleIds == null && input.RemoveResponsibleIds == null && input.CustomFieldsJson == null)
            throw new PluginMisconfigurationException("Supply at least one change for the selected tasks.");

        var request = new RestRequest("tasks/" + string.Join(",", input.TaskIds.Select(x => InputValidator.Id(x, "Task ID"))), Method.Put)
            .Optional("customStatus", input.CustomStatusId).Optional("addResponsibles", input.AddResponsibleIds).Optional("removeResponsibles", input.RemoveResponsibleIds);
        if (input.CustomFieldsJson != null)
            request.Optional("customFields", InputValidator.Json<List<Dictionary<string, string>>>(input.CustomFieldsJson, "Custom fields JSON"));

        var result = await Client.ExecuteWithErrorHandling<Envelope<TaskResponse>>(request);
        return new() { Tasks = result.Data };
    }

    private static RestRequest CreateRequest(CreateTaskRequest input)
    {
        InputValidator.Required(input.Title, "Title");
        ValidateStatus(input.Status, input.CustomStatusId);
        var request = new RestRequest($"folders/{InputValidator.Id(input.FolderId, "Folder ID")}/tasks", Method.Post)
            .Optional("title", input.Title).Optional("description", input.Description)
            .Optional("status", input.Status).Optional("customStatus", input.CustomStatusId)
            .Optional("responsibles", input.ResponsibleIds);
        if (input.StartDate.HasValue || input.DueDate.HasValue)
            request.Optional("dates", Dates(input.StartDate, input.DueDate));

        if (input.MetadataJson != null)
            request.Optional("metadata", InputValidator.Json<List<Dictionary<string, string?>>>(input.MetadataJson, "Metadata JSON"));

        return request;
    }

    private static Dictionary<string, object> Dates(DateTime? start, DateTime? due)
    {
        if (!due.HasValue || (start.HasValue && start > due))
            throw new PluginMisconfigurationException("Provide a due date on or after the start date.");

        var dates = new Dictionary<string, object> { ["type"] = start.HasValue ? "Planned" : "Milestone", ["due"] = due.Value.ToString("yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) };
        if (start.HasValue)
            dates["start"] = start.Value.ToString("yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

        return dates;
    }

    private static void ValidateStatus(string? status, string? customStatus)
    {
        if (status != null && customStatus != null)
            throw new PluginMisconfigurationException("Set either Status or Custom status ID, not both.");
    }
}
