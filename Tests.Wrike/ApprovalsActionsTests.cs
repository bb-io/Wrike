namespace Tests.Wrike;

[TestClass]
public class ApprovalsActionsTests : TestBase
{
    [DataTestMethod]
    [DataRow("GetApproval")]
    [DataRow("SearchApprovals")]
    [DataRow("CreateApproval")]
    [DataRow("UpdateApproval")]
    [DataRow("CancelApproval")]
    public Task ActionCompletesAgainstSandbox(string methodName)
        => RunAction(typeof(ApprovalsActions), methodName);
}
