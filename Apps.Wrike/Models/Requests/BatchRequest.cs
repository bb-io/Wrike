namespace Apps.Wrike.Models.Requests;

public class BatchRequest
{
    [Display("Operations JSON", Description = "JSON array of TaskCreate or FolderCreate operations, each with input and optional clientId. Maximum 100.")]
    public string OperationsJson { get; set; } = string.Empty;
}
