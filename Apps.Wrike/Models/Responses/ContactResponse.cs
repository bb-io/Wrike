namespace Apps.Wrike.Models.Responses;

public class ContactResponse
{
    [Display("User ID")]
    [JsonProperty("id")]
    public string UserId { get; set; } = string.Empty;

    [Display("First name")]
    [JsonProperty("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [Display("Last name")]
    [JsonProperty("lastName")]
    public string LastName { get; set; } = string.Empty;

    [Display("Primary email")]
    [JsonProperty("primaryEmail")]
    public string? PrimaryEmail { get; set; }

    [Display("Type")]
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;
}
