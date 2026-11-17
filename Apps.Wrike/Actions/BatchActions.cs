namespace Apps.Wrike.Actions;

[ActionList("Batches")]
public class BatchActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Submit batch", Description = "Submit up to 100 task or folder creations without resubmitting on failure.")]
    public async Task<BatchResponse> SubmitBatch([ActionParameter] BatchRequest input)
    {
        var operations = InputValidator.Json<List<Dictionary<string, object>>>(input.OperationsJson, "Operations JSON");
        if (operations.Count is < 1 or > 100)
            throw new PluginMisconfigurationException("Provide between 1 and 100 batch operations.");

        foreach (var operation in operations)
        {
            if (!operation.TryGetValue("type", out var type) || type.ToString() is not ("TaskCreate" or "FolderCreate") || !operation.ContainsKey("input"))
                throw new PluginMisconfigurationException("Each operation requires type TaskCreate or FolderCreate and an input object. Other batch types are not documented.");
        }
        var result = await Client.ExecuteWithErrorHandling<Envelope<JobDto>>(new RestRequest("batch", Method.Post).Optional("operations", operations));
        return Map(result.Data.First());
    }

    [Action("Get batch result", Description = "Read the current job state and individual results without replaying the batch.")]
    public async Task<BatchResponse> GetBatchResult([ActionParameter] JobIdentifier input)
    {
        var result = await Client.ExecuteWithErrorHandling<Envelope<JobDto>>(new RestRequest($"async_job/{InputValidator.Id(input.JobId, "Job ID")}"));
        return Map(result.Data.First());
    }

    [Action("Wait for batch", Description = "Wait up to 60 seconds for a job; return its ID and state if it is still processing.")]
    public async Task<BatchResponse> WaitForBatch([ActionParameter] JobIdentifier input)
    {
        for (var attempt = 0; attempt < 12; attempt++)
        {
            var result = await GetBatchResult(input);
            if (result.Status is "Completed" or "Failed" || attempt == 11)
                return result;

            await Task.Delay(TimeSpan.FromSeconds(5));
        }
        throw new PluginApplicationException("Could not read the batch result. Use Get batch result with the existing job ID.");
    }

    private static BatchResponse Map(JobDto job) => new()
    {
        JobId = job.JobId,
        Status = job.Status,
        ErrorMessage = job.ErrorMessage,
        ProgressPercent = job.ProgressPercent,
        ResultJson = job.Result == null ? null : JsonConvert.SerializeObject(job.Result)
    };
}
