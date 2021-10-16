using System.IO;

namespace NeoAgi.CommandLine
{
    /// <summary>
    /// 
    /// </summary>
    public static class GetOpsExtension
    {
        /// <summary>
        /// Helper method to parse string[] arguments into a POCO supplied by T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">A string[] to parse and merge into T.  Likely from Main().</param>
        /// <param name="output">A TextWriter to send output to.  System.Console.Out will be used by default.</param>
        /// <param name="func">An optional delegate to intercept before printing to output.  Return false to suppress the default render template.</param>
        /// <returns></returns>
        public static T GetOps<T>(this string[] args, TextWriter? output = null, Func<OptionManager, bool>? func = null) where T : new()
        {
            // Declare a few initial variabled
            T retVal = new T();
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

            // Do we need to print the help?
            bool printHelp = (raised != null);

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
            if (raised != null)
                throw raised;

            return retVal;
        }

        public static Dictionary<string, string> FlattenOpts<T>(this string[] args, string keyPrefix = "")
        {
            OptionManager manager = new OptionManager();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            
            dict = manager.Parse(args);

            return manager.Flatten<T>(keyPrefix, dict);
        }
    }
}
