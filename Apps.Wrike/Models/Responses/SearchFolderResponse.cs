namespace Apps.Wrike.Models.Responses;

public class SearchFolderResponse
{
    [Display("Folders")]
    public List<FolderResponse> Folders { get; set; } = [];
}
