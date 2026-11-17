namespace Tests.Wrike.Base;

public class TestUserDto
{
    [JsonProperty("profiles")]
    public List<TestUserProfileDto> Profiles { get; set; } = [];
}
