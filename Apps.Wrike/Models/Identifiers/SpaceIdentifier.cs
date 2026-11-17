namespace Apps.Wrike.Models.Identifiers;

public class SpaceIdentifier
{
    [Display("Space ID", Description = "Select a space to narrow discovery. Mapped IDs are accepted.")]
    [DataSource(typeof(SpaceHandler))]
    public string? SpaceId { get; set; }
}
