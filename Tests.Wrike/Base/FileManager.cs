using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Tests.Wrike.Base;

public class FileManager : IFileManagementClient
{
    public Task<Stream> DownloadAsync(FileReference reference)
        => Task.FromResult<Stream>(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Disposable Blackbird integration test attachment")));

    public async Task<FileReference> UploadAsync(Stream stream, string contentType, string fileName)
    {
        var folder = Path.Combine(AppContext.BaseDirectory, "TestFiles", "Output");
        Directory.CreateDirectory(folder);
        using var output = File.Create(Path.Combine(folder, Path.GetFileName(fileName)));
        await stream.CopyToAsync(output);
        return new() { Name = fileName, ContentType = contentType };
    }
}
