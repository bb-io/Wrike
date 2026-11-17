namespace Apps.Wrike.Models.Identifiers;

public class UserIdentifier
{
    [Display("User ID")]
    [DataSource(typeof(UserHandler))]
    public string UserId { get; set; } = string.Empty;
}
