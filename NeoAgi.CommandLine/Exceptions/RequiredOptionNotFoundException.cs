namespace NeoAgi.CommandLine.Exceptions
{
    public class RequiredOptionNotFoundException : ApplicationException
    {
        public OptionAttribute Option { get; set; }

        public RequiredOptionNotFoundException(OptionAttribute option)
        {
            Option = option;
        }
    }
}
