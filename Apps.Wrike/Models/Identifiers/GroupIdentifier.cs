namespace Apps.Wrike.Models.Identifiers;

public class GroupIdentifier
{
    [Display("Group ID")]
    [DataSource(typeof(GroupHandler))]
    public string GroupId { get; set; } = string.Empty;
}
