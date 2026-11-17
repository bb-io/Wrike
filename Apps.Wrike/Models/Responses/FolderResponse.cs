namespace Apps.Wrike.Models.Responses;

public class FolderResponse
{
    [Display("Folder ID")]
    [JsonProperty("id")]
    public string FolderId { get; set; } = string.Empty;

    [Display("Title")]
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [Display("Child IDs")]
    [JsonProperty("childIds")]
    public List<string> ChildIds { get; set; } = [];
}
