﻿namespace NeoAgi.CommandLine
{
    public static class StringArrayExtension
    {
        public static T? GetOps<T>(this string[] args, Action<OptionManager, ExitDirective>? func = null) where T : new()
        {
            bool invokeFunc = false;
            bool invokeMerge = false;
            OptionManager manager = new OptionManager();
            Dictionary<string, string> dict = new Dictionary<string, string>();

            try
            {
                dict = manager.Parse(args);
                invokeMerge = true;
            }
            catch (RaiseHelpException)
            {
                invokeFunc = true;
            }

            T retVal = new T();

            if (invokeMerge == true)
            {
                try
                {
                    retVal = OptionManager.Merge(retVal, dict);
                }
                catch (RequiredOptionNotFoundException ex)
                {
                    manager.Errors.Add(ex.Option);
                    invokeFunc = true;
                }
            }

            if (invokeFunc)
            {
                ExitDirective exit = new ExitDirective();
                if (func != null)
                {
                    func.Invoke(manager, exit);
                }

                if (exit.ProcessHelp)
                {
                    manager.PrintHelp<T>(Console.Out, manager.Errors);
                }

                if (exit.ExitCode != int.MinValue && exit.KillProcessOnError)
                {
                    Environment.Exit(exit.ExitCode);
                }

                return default(T);
            }

            return retVal;
        }
    }
}
