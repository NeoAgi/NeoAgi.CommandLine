using System.IO;

namespace NeoAgi.CommandLine
{
    public static class GetOpsExtension
    {
        public static T? GetOps<T>(this string[] args, TextWriter? output = null, Func<OptionManager, bool>? func = null) where T : new()
        {
            // Declare a few initial variabled
            T retVal = new T();
            bool printHelp = true;
            ApplicationException? raised = null;
            OptionManager manager = new OptionManager();
            Dictionary<string, string> dict = new Dictionary<string, string>();

            // First try to parse the arguments, if an exception is raised here, captured it but proceed on
            try
            {
                dict = manager.Parse(args);
            }
            catch (RaiseHelpException ex)
            {
                raised = ex;
            }

            // If we didn't error above, attempt to merge
            if (raised == null)
            {
                try
                {
                    retVal = OptionManager.Merge(retVal, dict);
                }
                catch (RequiredOptionNotFoundException ex)
                {
                    manager.Errors.Add(ex.Option);
                    raised = ex;
                }
            }

            // Regardless if an exception is raised, process the delegate
            if (func != null)
            {
                printHelp = func.Invoke(manager);
            }

            if (printHelp)
            {
                if (output == null)
                {
                    output = Console.Out;
                }

                manager.PrintHelp<T>(output, manager.Errors);
            }

            // If a exception is raised above, re-throw it now
            if(raised != null)
                throw raised;

            return retVal;
        }
    }
}
