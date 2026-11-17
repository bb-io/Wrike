namespace Apps.Wrike.Models.Dtos;

public class ErrorDto
{
    [JsonProperty("error")]
    public string? Error { get; set; }

    [JsonProperty("errorDescription")]
    public string? Description { get; set; }
}
