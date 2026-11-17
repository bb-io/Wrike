namespace Apps.Wrike.Models.Dtos;

public class JobDto : JobResponse
{
    [JsonProperty("result")]
    public object? Result { get; set; }
}
