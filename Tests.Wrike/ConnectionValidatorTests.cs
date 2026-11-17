namespace Tests.Wrike;

[TestClass]
public class ConnectionValidatorTests : TestBase
{
    [TestMethod]
    public async Task ConnectionReportsCurrentUser()
    {
        var result = await new Apps.Wrike.Connections.ConnectionValidator(InvocationContext)
            .ValidateConnection(InvocationContext.AuthenticationCredentialsProviders, CancellationToken.None);
        Assert.IsTrue(result.IsValid, result.Message);
        StringAssert.StartsWith(result.Message, "Connected as ");
    }
}
