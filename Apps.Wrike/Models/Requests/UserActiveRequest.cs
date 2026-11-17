namespace Apps.Wrike.Models.Requests;

public class UserActiveRequest : UserIdentifier
{
    [Display("Active")]
    public bool Active { get; set; }
}
