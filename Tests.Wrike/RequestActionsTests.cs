namespace Tests.Wrike;

[TestClass]
public class RequestActionsTests : TestBase
{
    [DataTestMethod]
    [DataRow("SendRequest")]
    public Task ActionCompletesAgainstSandbox(string methodName)
        => RunAction(typeof(RequestActions), methodName);
}
