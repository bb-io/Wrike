namespace Apps.Wrike.Models.Dtos;

public class TokenDto
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonProperty("host")]
    public string Host { get; set; } = string.Empty;
}
