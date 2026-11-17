namespace Apps.Wrike.Models.Requests;

public class GroupCreateRequest
{
    [Display("Title")]
    public string Title { get; set; } = string.Empty;

    [Display("Member IDs")]
    [DataSource(typeof(UserHandler))]
    public List<string>? MemberIds { get; set; }

    [Display("Parent group ID")]
    [DataSource(typeof(GroupHandler))]
    public string? ParentGroupId { get; set; }
}
