namespace Tests.Wrike;

[TestClass]
public class CommentsActionsTests : TestBase
{
    [DataTestMethod]
    [DataRow("SearchComments")]
    [DataRow("CreateComment")]
    [DataRow("UpdateComment")]
    [DataRow("DeleteComment")]
    public Task ActionCompletesAgainstSandbox(string methodName)
        => RunAction(typeof(CommentsActions), methodName);
}
