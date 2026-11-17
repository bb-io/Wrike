namespace Apps.Wrike.Models.Identifiers;

public class TimelogIdentifier : TaskChildIdentifier
{
    [Display("Timelog ID")]
    [DataSource(typeof(TimelogHandler))]
    public string TimelogId { get; set; } = string.Empty;
}
