namespace Apps.Wrike.Models.Requests;

public class EventInput : SpaceIdentifier
{
    [Display("Custom field ID")]
    [DataSource(typeof(CustomFieldHandler))]
    public string? CustomFieldId { get; set; }
}
