namespace Apps.Wrike.Models.Dtos;

public class Envelope<T>
{
    [JsonProperty("data")]
    public List<T> Data { get; set; } = [];

    [JsonProperty("nextPageToken")]
    public string? NextPageToken { get; set; }
}
