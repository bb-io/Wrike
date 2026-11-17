namespace Apps.Wrike.Models.Responses;

public class GroupResponse
{
    [Display("Group ID")]
    [JsonProperty("id")]
    public string GroupId { get; set; } = string.Empty;

    [Display("Title")]
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [Display("Member IDs")]
    [JsonProperty("memberIds")]
    public List<string> MemberIds { get; set; } = [];
}
