namespace NeoAgi.CommandLine
{
    /// <summary>
    /// Attribute to tag to Properties that defines how an option should be applied to the POCO
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : Attribute
    {
        /// <summary>
        /// Short name used to identify if an error occured while parsing.
        /// </summary>
        public string? FriendlyName { get; set; } = null;
        /// <summary>
        /// Short form option flag (e.g. -o), - will be appended automatically
        /// </summary>
        public string ShortName { get; set; } = string.Empty;
        /// <summary>
        /// Long form option flag (e.g. --option), -- will be appended automatically
        /// </summary>
        public string LongName { get; set; } = string.Empty;
        /// <summary>
        /// Concise description to be printed for usage assistance.
        /// </summary>
        public string Description {  get; set; } = string.Empty;
        /// <summary>
        /// Flag indicating if the option must be present for success.
        /// </summary>
        public bool Required { get; set; } = false;
    }
}
