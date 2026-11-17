namespace Tests.Wrike;

[TestClass]
public class AttachmentActionsTests : TestBase
{
    [DataTestMethod]
    [DataRow("SearchAttachments")]
    [DataRow("UploadAttachment")]
    [DataRow("DownloadAttachment")]
    [DataRow("DeleteAttachment")]
    public Task ActionCompletesAgainstSandbox(string methodName)
        => RunAction(typeof(AttachmentActions), methodName);
}
