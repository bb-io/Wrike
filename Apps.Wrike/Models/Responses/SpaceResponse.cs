namespace Apps.Wrike.Models.Responses;

public class SpaceResponse
{
    [Display("Space ID")]
    [JsonProperty("id")]
    public string SpaceId { get; set; } = string.Empty;

    [Display("Title")]
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [Display("Description")]
    [JsonProperty("description")]
    public string? Description { get; set; }

    [Display("Archived")]
    [JsonProperty("archived")]
    public bool Archived { get; set; }
}
