namespace Tests.Wrike;

[TestClass]
public class TaskActionsTests : TestBase
{
    [DataTestMethod]
    [DataRow("SearchTasks")]
    [DataRow("GetTask")]
    [DataRow("CreateTask")]
    [DataRow("CreateSubtask")]
    [DataRow("UpdateTask")]
    [DataRow("DeleteTask")]
    [DataRow("UpdateCustomField")]
    [DataRow("UpdateTasksInBulk")]
    public Task ActionCompletesAgainstSandbox(string methodName)
        => RunAction(typeof(TaskActions), methodName);
}
