public class Translation
{
    public Guid Id { get; set; }
    public Guid ObjectId { get; set; }
    public string FieldName { get; set; }
    public string LanguageCode { get; set; }
    public string TranslatedText { get; set; }
}