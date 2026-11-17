namespace Apps.Wrike.Models.Responses;

public class SearchAttachmentResponse
{
    [Display("Attachments")]
    public List<AttachmentResponse> Attachments { get; set; } = [];
}
