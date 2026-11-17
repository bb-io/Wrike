namespace Apps.Wrike.Models.Requests;

public class GroupUpdateRequest : GroupIdentifier
{
    [Display("Title")]
    public string? Title { get; set; }

    [Display("Add member IDs")]
    [DataSource(typeof(UserHandler))]
    public List<string>? AddMemberIds { get; set; }

    [Display("Remove member IDs")]
    [DataSource(typeof(UserHandler))]
    public List<string>? RemoveMemberIds { get; set; }
}
