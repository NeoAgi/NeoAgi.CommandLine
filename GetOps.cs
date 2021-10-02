using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoAgi.CommandLine
{
    public static class StringArrayExtension
    {
        public static T GetOps<T>(this string[] args, Action<OptionManager, OptionAttribute, ExitDirective>? func = null) where T : new()
        {
            OptionManager manager = new OptionManager();
            Dictionary<string, string> dict = manager.Parse(args);

            T retVal = new T();

            try
            {
                retVal = manager.Merge<T>(new T(), dict);
            }
            catch (RequiredOptionNotFoundException ex)
            {
                ExitDirective exit = new ExitDirective();
                if (func != null)
                {
                    func.Invoke(manager, ex.Option, exit);
                }

                if(exit.ProcessHelp)
                {
                    manager.PrintHelp<T>(Console.Out, ex.Option);
                }

                if (exit.ExitCode != int.MinValue)
                {
                    Environment.Exit(exit.ExitCode);
                }
            }

            return retVal;
        }
    }
}
