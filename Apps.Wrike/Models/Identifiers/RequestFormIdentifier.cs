namespace Apps.Wrike.Models.Identifiers;

public class RequestFormIdentifier : SpaceIdentifier
{
    [Display("Request form ID")]
    [DataSource(typeof(RequestFormHandler))]
    public string RequestFormId { get; set; } = string.Empty;
}
