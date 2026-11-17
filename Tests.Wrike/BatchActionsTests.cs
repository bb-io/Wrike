namespace Tests.Wrike;

[TestClass]
public class BatchActionsTests : TestBase
{
    [DataTestMethod]
    [DataRow("SubmitBatch")]
    [DataRow("GetBatchResult")]
    [DataRow("WaitForBatch")]
    public Task ActionCompletesAgainstSandbox(string methodName)
        => RunAction(typeof(BatchActions), methodName);
}
