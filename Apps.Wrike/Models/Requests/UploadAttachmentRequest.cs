namespace Apps.Wrike.Models.Requests;

public class UploadAttachmentRequest : TaskIdentifier
{
    [Display("File")]
    public Blackbird.Applications.Sdk.Common.Files.FileReference File { get; set; } = default!;
}
