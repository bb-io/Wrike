namespace Apps.Wrike.Models.Requests;

public class FieldValueRequest : TaskIdentifier
{
    [Display("Custom field ID")]
    [DataSource(typeof(CustomFieldHandler))]
    public string CustomFieldId { get; set; } = string.Empty;

    [Display("Value", Description = "Dropdown fields offer their configured values. Other types accept a mapped value; multiple-select values use a JSON array encoded as text.")]
    [DataSource(typeof(FieldOptionHandler))]
    public string Value { get; set; } = string.Empty;
}
