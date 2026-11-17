namespace Apps.Wrike.Models.Responses;

public class TimelogCategoryResponse
{
    [Display("Category ID")]
    [JsonProperty("id")]
    public string CategoryId { get; set; } = string.Empty;

    [Display("Name")]
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}
