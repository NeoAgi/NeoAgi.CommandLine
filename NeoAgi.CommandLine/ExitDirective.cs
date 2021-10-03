namespace NeoAgi.CommandLine
{
    public class ExitDirective
    {
        public int ExitCode { get; set; } = 1;
        public bool ProcessHelp { get; set; } = true;
        public bool KillProcessOnError { get; set; } = true;
    }
}
