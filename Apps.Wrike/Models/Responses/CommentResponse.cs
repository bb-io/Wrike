namespace Apps.Wrike.Models.Responses;

public class CommentResponse
{
    [Display("Comment ID")]
    [JsonProperty("id")]
    public string CommentId { get; set; } = string.Empty;

    [Display("Text")]
    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;

    [Display("Author ID")]
    [JsonProperty("authorId")]
    public string AuthorId { get; set; } = string.Empty;

    [Display("Created date")]
    [JsonProperty("createdDate")]
    public DateTime? CreatedDate { get; set; }
}
