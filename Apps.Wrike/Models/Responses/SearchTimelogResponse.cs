namespace Apps.Wrike.Models.Responses;

public class SearchTimelogResponse
{
    [Display("Timelogs")]
    public List<TimelogResponse> Timelogs { get; set; } = [];
}
