namespace Tests.Wrike;

[TestClass]
public class RequestFormsActionsTests : TestBase
{
    [DataTestMethod]
    [DataRow("SearchRequestForms")]
    [DataRow("GetRequestForm")]
    [DataRow("SubmitRequestForm")]
    [DataRow("CreatePrefilledFormUrl")]
    public Task ActionCompletesAgainstSandbox(string methodName)
        => RunAction(typeof(RequestFormsActions), methodName);
}
