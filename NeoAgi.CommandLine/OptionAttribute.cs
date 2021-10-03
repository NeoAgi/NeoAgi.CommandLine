namespace NeoAgi.CommandLine
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : Attribute
    {
        public string? FriendlyName { get; set; } = null;
        public string ShortName { get; set; } = string.Empty;
        public string LongName { get; set; } = string.Empty;
        public string Description {  get; set; } = string.Empty;
        public bool Required { get; set; } = false;
        public string NotFoundText { get; set; } = string.Empty;
    }
}
