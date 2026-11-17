namespace Apps.Wrike.Models.Requests;

public class FolderSearchRequest : SpaceIdentifier
{
    [Display("Search term", Description = "Find folders whose names contain this text. Leave blank to list folders. Text is treated as one phrase, not a comma-separated list or expression.")]
    public string? SearchTerm { get; set; }
}
