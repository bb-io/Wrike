namespace Apps.Wrike.Models.Responses;

public class SearchTaskResponse
{
    [Display("Tasks")]
    public List<TaskResponse> Tasks { get; set; } = [];
}
