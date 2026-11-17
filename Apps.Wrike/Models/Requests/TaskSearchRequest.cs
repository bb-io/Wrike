namespace Apps.Wrike.Models.Requests;

public class TaskSearchRequest : SpaceIdentifier
{
    [Display("Folder ID")]
    [DataSource(typeof(FolderHandler))]
    public string? FolderId { get; set; }

    [Display("Title")]
    public string? Title { get; set; }

    [Display("Include subtasks")]
    public bool? IncludeSubtasks { get; set; }

    [Display("Maximum results")]
    public int? MaximumResults { get; set; }
}
