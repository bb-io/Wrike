namespace Apps.Wrike.Models.Responses;

public class SearchCustomFieldResponse
{
    [Display("Custom fields")]
    public List<CustomFieldResponse> CustomFields { get; set; } = [];
}
