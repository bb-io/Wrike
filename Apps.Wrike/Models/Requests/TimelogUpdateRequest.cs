namespace Apps.Wrike.Models.Requests;

public class TimelogUpdateRequest : TimelogIdentifier
{
    [Display("Hours")]
    public double? Hours { get; set; }

    [Display("Tracked date")]
    public DateTime? TrackedDate { get; set; }

    [Display("Comment")]
    public string? Comment { get; set; }

    [Display("Category ID")]
    [DataSource(typeof(TimelogCategoryHandler))]
    public string? CategoryId { get; set; }
}
