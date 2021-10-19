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
        /// <returns></returns>
        public static T GetOpts<T>(this string[] args) where T : new()
        {
            return args.GetOpts<T>(null);
        }

        /// <summary>
        /// Helper method to parse string[] arguments into a POCO supplied by T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">A string[] to parse and merge into T.  Likely from Main().</param>
        /// <param name="output">A TextWriter to send output to.  System.Console.Out will be used by default.</param>
        /// <returns></returns>
        public static T GetOpts<T>(this string[] args, TextWriter? output = null) where T : new()
        {
            // Declare a few initial variabled
            T retVal = new T();
            OptionManager manager = new OptionManager();

            try
            {
                return OptionManager.Merge(retVal, manager.Parse(args));
            }
            catch (RequiredOptionNotFoundException ex)
            {
                manager.Errors.Add(ex.Option);
                if (output != null)
                    manager.PrintHelp<T>(output, manager.Errors);

                throw;
            }
            catch (RaiseHelpException)
            {
                if (output != null)
                    manager.PrintHelp<T>(output);

                throw;
            }
        }

        /// <summary>
        /// Helper method to parse string[] arguments into a Dictionary&lt;string, string&gt; supplied by T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="keyPrefix"></param>
        /// <param name="output">A TextWriter to send output to.  System.Console.Out will be used by default.</param>
        /// <returns></returns>
        public static Dictionary<string, string> FlattenOpts<T>(this string[] args, string keyPrefix = "", TextWriter? output = null) where T : new()
        {
            // Declare a few initial variabled
            T retVal = new T();
            OptionManager manager = new OptionManager();
            Dictionary<string, string> dict = new Dictionary<string, string>();

            try
            {
                return manager.Flatten<T>(keyPrefix, manager.Parse(args));
            }
            catch (RequiredOptionNotFoundException ex)
            {
                manager.Errors.Add(ex.Option);
                if (output != null)
                    manager.PrintHelp<T>(output, manager.Errors);

                throw;
            }
            catch (RaiseHelpException)
            {
                if (output != null)
                    manager.PrintHelp<T>(output);

                throw;
            }
        }
    }
}
