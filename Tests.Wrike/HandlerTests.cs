using Apps.Wrike.Handlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Tests.Wrike;

[TestClass]
public class HandlerTests : TestBase
{
    [DataTestMethod]
    [DataRow("Space")]
    [DataRow("Folder")]
    [DataRow("Task")]
    [DataRow("Workflow")]
    [DataRow("Status")]
    [DataRow("CustomField")]
    [DataRow("FieldOption")]
    [DataRow("User")]
    [DataRow("Group")]
    [DataRow("RequestForm")]
    [DataRow("TimelogCategory")]
    [DataRow("Comment")]
    [DataRow("Attachment")]
    [DataRow("Timelog")]
    [DataRow("Approval")]
    public async Task DropdownReturnsUniqueStoredIds(string name)
    {
        var space = new SpaceIdentifier { SpaceId = Setting("SandboxSpaceId") };
        var tasks = await new TaskActions(InvocationContext).SearchTasks(new TaskSearchRequest { FolderId = Setting("SandboxFolderId"), MaximumResults = 1 });
        var task = new TaskIdentifier { SpaceId = space.SpaceId, TaskId = tasks.Tasks.First().TaskId };
        var workflows = await new WorkflowHandler(InvocationContext, space).GetDataAsync(new DataSourceContext(), CancellationToken.None);
        var workflow = new WorkflowIdentifier { SpaceId = space.SpaceId, WorkflowId = workflows.First().Value };
        IAsyncDataSourceItemHandler handler = name switch
        {
            "Space" => new SpaceHandler(InvocationContext),
            "Folder" => new FolderHandler(InvocationContext, space),
            "Task" => new TaskHandler(InvocationContext, new TaskSearchRequest { SpaceId = space.SpaceId }),
            "Workflow" => new WorkflowHandler(InvocationContext, space),
            "Status" => new StatusHandler(InvocationContext, workflow),
            "CustomField" => new CustomFieldHandler(InvocationContext, space),
            "FieldOption" => new FieldOptionHandler(InvocationContext, new CustomFieldIdentifier { CustomFieldId = Setting("SandboxCustomFieldId") }),
            "User" => new UserHandler(InvocationContext),
            "Group" => new GroupHandler(InvocationContext),
            "RequestForm" => new RequestFormHandler(InvocationContext, space),
            "TimelogCategory" => new TimelogCategoryHandler(InvocationContext),
            "Comment" => new CommentHandler(InvocationContext, task),
            "Attachment" => new AttachmentHandler(InvocationContext, task),
            "Timelog" => new TimelogHandler(InvocationContext, task),
            "Approval" => new ApprovalHandler(InvocationContext, task),
            _ => throw new ArgumentException(name)
        };
        var result = (await handler.GetDataAsync(new DataSourceContext(), CancellationToken.None)).ToList();
        Assert.AreEqual(result.Count, result.Select(x => x.Value).Distinct().Count());
        Assert.IsTrue(result.All(x => !string.IsNullOrWhiteSpace(x.Value) && !string.IsNullOrWhiteSpace(x.DisplayName)));
        var none = await handler.GetDataAsync(new DataSourceContext { SearchString = "blackbird-no-match-" + Guid.NewGuid().ToString("N") }, CancellationToken.None);
        Assert.AreEqual(0, none.Count());
    }

    [TestMethod]
    public async Task FolderSuggestionsWorkWithoutSpaceAndWithSpace()
    {
        var title = "Blackbird picker " + Guid.NewGuid().ToString("N");
        var created = await Client.ExecuteWithErrorHandling<Envelope<FolderResponse>>(
            new RestRequest($"folders/{Setting("SandboxFolderId")}/folders", Method.Post).Optional("title", title));
        var folder = created.Data.First();
        try
        {
            foreach (var space in new[] { new SpaceIdentifier(), new SpaceIdentifier { SpaceId = Setting("SandboxSpaceId") } })
            {
                var handler = new FolderHandler(InvocationContext, space);
                var result = (await handler.GetDataAsync(new DataSourceContext { SearchString = title }, CancellationToken.None)).ToList();
                Assert.IsTrue(result.Any(x => x.Value == folder.FolderId && x.DisplayName.Contains(title)));
                var none = await handler.GetDataAsync(new DataSourceContext { SearchString = Guid.NewGuid().ToString("N") }, CancellationToken.None);
                Assert.AreEqual(0, none.Count());
            }
        }
        finally
        {
            await Client.ExecuteWithErrorHandling(new RestRequest($"folders/{folder.FolderId}", Method.Delete));
        }
    }

    [TestMethod]
    public async Task TaskSuggestionsWorkWithoutParentAndWithSpace()
    {
        var title = "Blackbird task picker " + Guid.NewGuid().ToString("N");
        var actions = new TaskActions(InvocationContext);
        var task = await actions.CreateTask(new CreateTaskRequest { FolderId = Setting("SandboxFolderId"), Title = title });
        try
        {
            foreach (var scope in new[] { new TaskSearchRequest(), new TaskSearchRequest { SpaceId = Setting("SandboxSpaceId") }, new TaskSearchRequest { FolderId = Setting("SandboxFolderId") } })
            {
                var handler = new TaskHandler(InvocationContext, scope);
                var matches = await handler.GetDataAsync(new DataSourceContext { SearchString = title }, CancellationToken.None);
                Assert.IsTrue(matches.Any(x => x.Value == task.TaskId && x.DisplayName.Contains(title)));
                var none = await handler.GetDataAsync(new DataSourceContext { SearchString = Guid.NewGuid().ToString("N") }, CancellationToken.None);
                Assert.AreEqual(0, none.Count());
            }
        }
        finally
        {
            await Client.ExecuteWithErrorHandling(new RestRequest($"tasks/{task.TaskId}", Method.Delete));
        }
    }
}
