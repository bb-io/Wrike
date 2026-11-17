namespace Tests.Wrike;

[TestClass]
public class TimelogsActionsTests : TestBase
{
    [DataTestMethod]
    [DataRow("SearchTimelogs")]
    [DataRow("CreateTimelog")]
    [DataRow("UpdateTimelog")]
    [DataRow("DeleteTimelog")]
    public Task ActionCompletesAgainstSandbox(string methodName)
        => RunAction(typeof(TimelogsActions), methodName);
}
