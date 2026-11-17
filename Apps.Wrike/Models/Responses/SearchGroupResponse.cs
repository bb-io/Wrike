namespace Apps.Wrike.Models.Responses;

public class SearchGroupResponse
{
    [Display("Groups")]
    public List<GroupResponse> Groups { get; set; } = [];
}
