namespace Apps.Wrike.Models.Identifiers;

public class FolderIdentifier : SpaceIdentifier
{
    [Display("Folder ID", Description = "Search accessible folders by name. Optionally select Space ID to narrow suggestions. Manual and mapped folder IDs are accepted.")]
    [DataSource(typeof(FolderHandler))]
    public string FolderId { get; set; } = string.Empty;
}
