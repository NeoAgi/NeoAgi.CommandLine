using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace NeoAgi.CommandLine
{
    public class OptionManager
    {
        public OptionManager() { }

        public Dictionary<string, string> Parse(string[] arr)
        {
            Dictionary<string, string> tuples = new Dictionary<string, string>();

            for (int i = 0; i < arr.Length; i++)
            {
                // Look to see if this string starts with a -, and has a value after it
                if (arr[i].StartsWith('-') && arr.Length >= i + 1)
                {
                    tuples.Add(arr[i], arr[i + 1]);
                    i++;
                }
            }

            return tuples;
        }

        public T Merge<T>(T ret, Dictionary<string, string> values)
        {
            Type typeT = typeof(T);
            foreach (PropertyInfo prop in typeT.GetProperties())
            {
                foreach (OptionAttribute attr in prop.GetCustomAttributes(typeof(OptionAttribute)))
                {
                    bool propFound = false;

                    if (values.ContainsKey($"-{attr.ShortName}"))
                    {
                        propFound = true;
                        prop.SetValue(ret, values[$"-{attr.ShortName}"]);
                    }

                    if (values.ContainsKey($"--{attr.LongName}"))
                    {
                        propFound = true;
                        prop.SetValue(ret, values[$"--{attr.LongName}"]);
                    }

                    if (!propFound && attr.Required)
                        throw new RequiredOptionNotFoundException(attr);
                }
            }

            return ret;
        }

        public void PrintHelp<T>(TextWriter stdout, OptionAttribute? error = null) where T : new()
        {
            PrintHelpHeader(stdout);
            if(error != null) PrintHelpErrors(stdout, error);
            PrintHelpOptions<T>(stdout);
        }

        public void PrintHelpHeader(TextWriter stdout)
        {
            Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            stdout.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} v{assembly.GetName().Version}");
            if (!string.IsNullOrWhiteSpace(fileInfo.CompanyName) && !string.IsNullOrWhiteSpace(fileInfo.LegalCopyright))
            {
                stdout.WriteLine($"{fileInfo.CompanyName} - {fileInfo.LegalCopyright}");
            }

            stdout.WriteLine();
        }

        public void PrintHelpErrors(TextWriter stdout, OptionAttribute error)
        {
            stdout.WriteLine("ERROR(s):");
            stdout.WriteLine($"\tThe required option '{error.FriendlyName}' was not provided.");
            stdout.WriteLine();
        }

        public void PrintHelpOptions<T>(TextWriter stdout) where T : new()
        {
            stdout.WriteLine("Options:");

            Dictionary<string, string> optionHash = new Dictionary<string, string>();
            int maxKeyLen = 0;

            Type typeT = typeof(T);
            foreach (PropertyInfo prop in typeT.GetProperties())
            {
                foreach (OptionAttribute attr in prop.GetCustomAttributes(typeof(OptionAttribute)))
                {
                    string key = string.Empty;
                    if (!string.IsNullOrEmpty(attr.ShortName) && string.IsNullOrEmpty(attr.LongName))
                        key = $"-{attr.ShortName}";

                    if (string.IsNullOrEmpty(attr.ShortName) && !string.IsNullOrEmpty(attr.LongName))
                        key = $"--{attr.LongName}";

                    if (!string.IsNullOrEmpty(attr.ShortName) && !string.IsNullOrEmpty(attr.LongName))
                        key = $"-{attr.ShortName}, --{attr.LongName}";

                    if (key.Length > maxKeyLen)
                        maxKeyLen = key.Length;

                    string desc = (attr.Required)
                        ? attr.Description
                        : $"{attr.Description} (optional)";

                    optionHash.Add(key, desc);
                }
            }

            foreach (KeyValuePair<string, string> pair in optionHash)
            {
                stdout.WriteLine($"{pair.Key.PadRight(maxKeyLen, ' ')}\t| {pair.Value}");
            }

            stdout.WriteLine();
        }
    }
}
