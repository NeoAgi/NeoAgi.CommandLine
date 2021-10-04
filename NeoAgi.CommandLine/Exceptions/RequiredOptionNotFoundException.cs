namespace NeoAgi.CommandLine.Exceptions
{
    /// <summary>
    /// Raised when an option is marked required yet not satisified with the aruments provided
    /// </summary>
    public class RequiredOptionNotFoundException : ApplicationException
    {
        /// <summary>
        /// Option that failed to be satisfied.
        /// </summary>
        public OptionAttribute Option { get; set; }

        /// <summary>
        /// Required Constructor that accepts the offending option
        /// </summary>
        /// <param name="option"></param>
        public RequiredOptionNotFoundException(OptionAttribute option)
        {
            Option = option;
        }
    }
}
