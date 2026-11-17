namespace Apps.Wrike.Models.Responses;

public class SearchContactResponse
{
    [Display("Contacts")]
    public List<ContactResponse> Contacts { get; set; } = [];
}
