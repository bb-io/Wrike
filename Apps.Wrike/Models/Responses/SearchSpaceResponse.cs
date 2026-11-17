namespace Apps.Wrike.Models.Responses;

public class SearchSpaceResponse
{
    [Display("Spaces")]
    public List<SpaceResponse> Spaces { get; set; } = [];
}
