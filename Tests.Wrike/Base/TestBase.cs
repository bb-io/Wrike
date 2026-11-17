using System.Reflection;
using Apps.Wrike.Api;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Microsoft.Extensions.Configuration;

namespace Tests.Wrike.Base;

public class TestBase
{
    protected readonly InvocationContext InvocationContext;
    protected readonly WrikeClient Client;
    protected readonly FileManager FileManager = new();

    public TestContext TestContext { get; set; } = default!;

    public TestBase()
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        var credentials = config.GetSection("ConnectionDefinition").GetChildren()
            .Select(x => new AuthenticationCredentialsProvider(x.Key, x.Value ?? string.Empty)).ToArray();
        InvocationContext = new() { AuthenticationCredentialsProviders = credentials };
        Client = new(credentials);
    }

    protected string Setting(string key)
    {
        var value = TestContext.Properties[key]?.ToString();
        Assert.IsFalse(string.IsNullOrWhiteSpace(value), $"Set {key} in the local sandbox.runsettings file.");
        return value!;
    }

    protected async Task RunAction(Type type, string methodName)
    {
        var spaceId = Setting("SandboxSpaceId");
        var parentFolder = Setting("SandboxFolderId");
        var folderResult = await Client.ExecuteWithErrorHandling<Envelope<FolderResponse>>(new RestRequest($"folders/{parentFolder}/folders", Method.Post)
            .Optional("title", "Blackbird disposable test " + Guid.NewGuid().ToString("N")));
        var folderId = folderResult.Data.First().FolderId;
        string? groupId = null;
        string? submittedTask = null;
        string? submittedProject = null;
        try
        {
            var tasks = new TaskActions(InvocationContext);
            var task = await tasks.CreateTask(new CreateTaskRequest { FolderId = folderId, Title = "Disposable integration test" });
            var values = new Dictionary<string, object?>
            {
                ["SpaceId"] = spaceId,
                ["FolderId"] = folderId,
                ["TaskId"] = task.TaskId,
                ["TaskIds"] = new[] { task.TaskId },
                ["ParentTaskId"] = task.TaskId,
                ["Title"] = "Disposable integration test updated",
                ["Description"] = "Temporary test data",
                ["Text"] = "Temporary comment",
                ["Hours"] = 0.25,
                ["TrackedDate"] = DateTime.UtcNow.Date,
                ["Comment"] = "Temporary timelog",
                ["File"] = new FileReference { Name = "test.txt", ContentType = "text/plain" },
                ["Method"] = "GET",
                ["Path"] = "contacts",
                ["ParametersJson"] = "{\"me\":true}",
                ["MaximumResults"] = 100,
                ["Value"] = "Blackbird test"
            };
            if (type == typeof(CommentsActions))
            {
                var comment = await new CommentsActions(InvocationContext).CreateComment(new CommentCreateRequest { TaskId = task.TaskId, Text = "Temporary comment" });
                values["CommentId"] = comment.CommentId;
            }
            if (type == typeof(TimelogsActions))
            {
                var timelog = await new TimelogsActions(InvocationContext).CreateTimelog(new TimelogCreateRequest { TaskId = task.TaskId, Hours = 0.25, TrackedDate = DateTime.UtcNow.Date, Comment = "Temporary timelog" });
                values["TimelogId"] = timelog.TimelogId;
            }
            if (type == typeof(ApprovalsActions))
            {
                var approval = await new ApprovalsActions(InvocationContext).CreateApproval(new ApprovalCreateRequest { TaskId = task.TaskId, Description = "Disposable draft" });
                values["ApprovalId"] = approval.ApprovalId;
            }
            if (type == typeof(AttachmentActions))
            {
                var attachment = await new AttachmentActions(InvocationContext, FileManager).UploadAttachment(new UploadAttachmentRequest { TaskId = task.TaskId, File = (FileReference)values["File"]! });
                values["AttachmentId"] = attachment.AttachmentId;
            }
            if (type == typeof(AdministrationActions))
            {
                Assert.AreEqual("true", Setting("AllowAdministration"), "Use a disposable account for administration tests.");
                var group = await new AdministrationActions(InvocationContext).CreateGroup(new GroupCreateRequest { Title = "Blackbird disposable " + Guid.NewGuid().ToString("N") });
                groupId = group.GroupId;
                values["GroupId"] = groupId;
                values["UserId"] = Setting("SandboxUserId");
                var user = await Client.ExecuteWithErrorHandling<Envelope<TestUserDto>>(new RestRequest($"users/{values["UserId"]}"));
                Assert.IsTrue(user.Data.First().Profiles.All(x => x.Active), "Sandbox user must already be active; this test does not activate disabled users.");
                values["Active"] = true;
            }
            if (type == typeof(RequestFormsActions))
            {
                Assert.AreEqual("true", Setting("AllowFormSubmission"), "Use a disposable request form without production routing.");
                values["RequestFormId"] = Setting("SandboxRequestFormId");
                values["AnswersJson"] = Setting("SandboxFormAnswers");
            }
            if (methodName == "UpdateCustomField")
                values["CustomFieldId"] = Setting("SandboxCustomFieldId");

            if (methodName == "UpdateTasksInBulk")
            {
                var me = await Client.ExecuteWithErrorHandling<Envelope<ContactResponse>>(new RestRequest("contacts").Optional("me", true));
                values["AddResponsibleIds"] = new[] { me.Data.First().UserId };
            }
            if (type == typeof(BatchActions))
            {
                var json = JsonConvert.SerializeObject(new[] { new { type = "TaskCreate", clientId = "test-operation", input = new { folderId, title = "Disposable batch task" } } });
                values["OperationsJson"] = json;
                if (methodName != "SubmitBatch")
                {
                    var job = await new BatchActions(InvocationContext).SubmitBatch(new BatchRequest { OperationsJson = json });
                    values["JobId"] = job.JobId;
                }
            }
            object instance = type == typeof(AttachmentActions)
                ? new AttachmentActions(InvocationContext, FileManager)
                : Activator.CreateInstance(type, InvocationContext)!;
            var method = type.GetMethod(methodName)!;
            var parameter = method.GetParameters().FirstOrDefault();
            var arguments = parameter == null ? Array.Empty<object>() : new[] { JsonConvert.DeserializeObject(JsonConvert.SerializeObject(values), parameter.ParameterType)! };
            var operation = (Task)method.Invoke(instance, arguments)!;
            await operation;
            var result = operation.GetType().GetProperty("Result")?.GetValue(operation);
            if (method.ReturnType.IsGenericType)
                Assert.IsNotNull(result);

            if (result is GroupResponse created && methodName == "CreateGroup")
                await Client.ExecuteWithErrorHandling(new RestRequest($"groups/{created.GroupId}", Method.Delete));

            if (methodName == "DeleteGroup")
                groupId = null;

            if (result is FormSubmitResponse submitted)
            {
                submittedTask = submitted.TaskId;
                submittedProject = submitted.ProjectId;
            }
            if (type == typeof(BatchActions))
            {
                var jobId = (result as BatchResponse)?.JobId ?? values["JobId"]?.ToString();
                var completed = await new BatchActions(InvocationContext).WaitForBatch(new JobIdentifier { JobId = jobId! });
                Assert.AreEqual("Completed", completed.Status, "Inspect the existing job before running this test again.");
                Assert.IsFalse(string.IsNullOrWhiteSpace(completed.ResultJson));
            }
        }
        finally
        {
            if (submittedTask != null)
                await Client.ExecuteWithErrorHandling(new RestRequest($"tasks/{submittedTask}", Method.Delete));

            if (submittedProject != null)
                await Client.ExecuteWithErrorHandling(new RestRequest($"folders/{submittedProject}", Method.Delete));

            if (groupId != null)
                await Client.ExecuteWithErrorHandling(new RestRequest($"groups/{groupId}", Method.Delete));

            await Client.ExecuteWithErrorHandling(new RestRequest($"folders/{folderId}", Method.Delete));
        }
    }
}
