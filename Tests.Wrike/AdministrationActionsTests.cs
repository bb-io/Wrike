namespace Tests.Wrike;

[TestClass]
public class AdministrationActionsTests : TestBase
{
    [DataTestMethod]
    [DataRow("SearchGroups")]
    [DataRow("CreateGroup")]
    [DataRow("UpdateGroup")]
    [DataRow("DeleteGroup")]
    [DataRow("GetUser")]
    [DataRow("UpdateUserActiveStatus")]
    public Task ActionCompletesAgainstSandbox(string methodName)
        => RunAction(typeof(AdministrationActions), methodName);
}
