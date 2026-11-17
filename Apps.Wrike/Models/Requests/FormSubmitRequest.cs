namespace Apps.Wrike.Models.Requests;

public class FormSubmitRequest : RequestFormIdentifier
{
    [Display("Answers JSON", Description = "JSON array: [{fieldId: ..., values: [...]}], with quoted keys. Inspect the selected form first.")]
    public string AnswersJson { get; set; } = string.Empty;
}
