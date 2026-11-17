namespace Tests.Wrike;

[TestClass]
public class DiscoveryActionsTests : TestBase
{
    [DataTestMethod]
    [DataRow("SearchSpaces")]
    [DataRow("SearchFolders")]
    [DataRow("SearchWorkflows")]
    [DataRow("SearchCustomFields")]
    [DataRow("SearchUsers")]
    public Task ActionCompletesAgainstSandbox(string methodName)
        => RunAction(typeof(DiscoveryActions), methodName);

    [TestMethod]
    public async Task SearchFoldersFiltersNamesInSpaceAndAccount()
    {
        var title = "Blackbird search " + Guid.NewGuid().ToString("N");
        var created = await Client.ExecuteWithErrorHandling<Envelope<FolderResponse>>(
            new RestRequest($"folders/{Setting("SandboxFolderId")}/folders", Method.Post).Optional("title", title));
        var folder = created.Data.First();
        try
        {
            var actions = new DiscoveryActions(InvocationContext);
            var scoped = await actions.SearchFolders(new FolderSearchRequest { SpaceId = Setting("SandboxSpaceId"), SearchTerm = title });
            Assert.IsTrue(scoped.Folders.Any(x => x.FolderId == folder.FolderId));
            var account = await actions.SearchFolders(new FolderSearchRequest { SearchTerm = title });
            Assert.IsTrue(account.Folders.Any(x => x.FolderId == folder.FolderId));
            var empty = await actions.SearchFolders(new FolderSearchRequest { SpaceId = Setting("SandboxSpaceId"), SearchTerm = Guid.NewGuid().ToString("N") });
            Assert.AreEqual(0, empty.Folders.Count);
        }
        finally
        {
            await Client.ExecuteWithErrorHandling(new RestRequest($"folders/{folder.FolderId}", Method.Delete));
        }
    }
}
