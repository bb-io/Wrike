namespace Apps.Wrike.Models.Identifiers;

public class CustomFieldIdentifier : SpaceIdentifier
{
    [Display("Custom field ID")]
    [DataSource(typeof(CustomFieldHandler))]
    public string CustomFieldId { get; set; } = string.Empty;
}
