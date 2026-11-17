namespace Apps.Wrike.Models.Responses;

public class FileResponse
{
    [Display("File")]
    public Blackbird.Applications.Sdk.Common.Files.FileReference File { get; set; } = default!;
}
