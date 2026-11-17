using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;

namespace Apps.Wrike.Actions;

[ActionList("Attachments")]
public class AttachmentActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
{
    [Action("Search attachments", Description = "Find attachments on a task.")]
    public async Task<SearchAttachmentResponse> SearchAttachments([ActionParameter] TaskIdentifier input)
        => new() { Attachments = await Client.Paginate<AttachmentResponse>(() => new RestRequest($"tasks/{InputValidator.Id(input.TaskId, "Task ID")}/attachments")) };

    [Action("Upload attachment", Description = "Attach a file to a task.")]
    public async Task<AttachmentResponse> UploadAttachment([ActionParameter] UploadAttachmentRequest input)
    {
        InputValidator.Required(input.TaskId, "Task ID");
        if (input.File == null || string.IsNullOrWhiteSpace(input.File.Name))
            throw new PluginMisconfigurationException("Select a file with a filename.");

        try
        {
            using var stream = await fileManagementClient.DownloadAsync(input.File);
            var bytes = await stream.GetByteData();
            var request = new RestRequest($"tasks/{InputValidator.Id(input.TaskId, "Task ID")}/attachments", Method.Post)
                .AddHeader("X-File-Name", Uri.EscapeDataString(input.File.Name))
                .AddHeader("X-Requested-With", "XMLHttpRequest")
                .AddParameter("application/octet-stream", bytes, ParameterType.RequestBody);
            var result = await Client.ExecuteWithErrorHandling<Envelope<AttachmentResponse>>(request);
            return result.Data.First();
        }
        catch (Exception ex) when (ex is HttpRequestException or IOException or TaskCanceledException)
        {
            throw new PluginApplicationException("Could not transfer the file. Check file availability and whether the attachment was created before retrying.");
        }
    }

    [Action("Download attachment", Description = "Download a native attachment as a reusable file.")]
    public async Task<FileResponse> DownloadAttachment([ActionParameter] AttachmentIdentifier input)
    {
        var metadata = await Client.ExecuteWithErrorHandling<Envelope<AttachmentResponse>>(new RestRequest($"attachments/{InputValidator.Id(input.AttachmentId, "Attachment ID")}"));
        var attachment = metadata.Data.First();
        if (attachment.Type != "Wrike")
            throw new PluginMisconfigurationException("This is an external attachment. Download it using the app for its storage provider.");

        var response = await Client.ExecuteWithErrorHandling(new RestRequest($"attachments/{InputValidator.Id(input.AttachmentId, "Attachment ID")}/download"));
        try
        {
            using var stream = new MemoryStream(response.RawBytes ?? []);
            return new() { File = await fileManagementClient.UploadAsync(stream, attachment.ContentType ?? "application/octet-stream", attachment.Name) };
        }
        catch (Exception ex) when (ex is HttpRequestException or IOException or TaskCanceledException)
        {
            throw new PluginApplicationException("Could not store the downloaded file. Check file service availability and try again.");
        }
    }

    [Action("Delete attachment", Description = "Delete a selected attachment.")]
    public async Task DeleteAttachment([ActionParameter] AttachmentIdentifier input)
        => await Client.ExecuteWithErrorHandling(new RestRequest($"attachments/{InputValidator.Id(input.AttachmentId, "Attachment ID")}", Method.Delete));
}
