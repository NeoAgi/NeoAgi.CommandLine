﻿using System;
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
        public List<OptionAttribute> Errors { get; set; } = new List<OptionAttribute>();

        protected static Dictionary<Type, Dictionary<PropertyInfo, OptionAttribute>> _reflectCache = new Dictionary<Type, Dictionary<PropertyInfo, OptionAttribute>>();

        public OptionManager() { }

        public Dictionary<string, string> Parse(string[] arr)
        {
            Dictionary<string, string> tuples = new Dictionary<string, string>();

            for (int i = 0; i < arr.Length; i++)
            {
                // Special case, if the first paramater is --help, bail early and print
                if (i == 0 && arr[i].Equals("--help"))
                    throw new RaiseHelpException();

                // Look to see if this string starts with a -, and has a value after it
                if (arr[i].StartsWith('-') && arr.Length >= i + 1)
                {
                    tuples.Add(arr[i], arr[i + 1]);
                    i++;
                }
            }

            return tuples;
        }

        public static T Merge<T>(T ret, Dictionary<string, string> values)
        {
            Dictionary<PropertyInfo, OptionAttribute> propBag = ReflectType<T>();
            foreach (KeyValuePair<PropertyInfo, OptionAttribute> kvp in propBag)
            {
                PropertyInfo prop = kvp.Key;
                OptionAttribute attr = kvp.Value;

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

            return ret;
        }

        public void PrintHelp<T>(TextWriter stdout, IEnumerable<OptionAttribute>? errors = null) where T : new()
        {
            PrintHelpHeader(stdout);
            if (errors != null) PrintHelpErrors(stdout, errors);
            PrintHelpOptions<T>(stdout);
        }

        public static void PrintHelpHeader(TextWriter stdout)
        {
            Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            stdout.WriteLine($"USAGE: {AppDomain.CurrentDomain.FriendlyName} v{assembly.GetName().Version}");
            if (!string.IsNullOrWhiteSpace(fileInfo.CompanyName) && !string.IsNullOrWhiteSpace(fileInfo.LegalCopyright))
            {
                stdout.WriteLine($"{fileInfo.CompanyName} - {fileInfo.LegalCopyright}");
            }

            stdout.WriteLine();
        }

        public static void PrintHelpErrors(TextWriter stdout, IEnumerable<OptionAttribute> errors)
        {
            if (errors.Count() > 0)
            {
                stdout.WriteLine("ERROR(s):");
                foreach (OptionAttribute error in errors)
                {
                    stdout.WriteLine($"\tThe required option '{error.FriendlyName}' was not provided.");
                }
                stdout.WriteLine();
            }
        }

        public static void PrintHelpOptions<T>(TextWriter stdout) where T : new()
        {
            stdout.WriteLine("Options:");

            Dictionary<string, string> optionHash = new Dictionary<string, string>();
            int maxKeyLen = 0;

            Dictionary<PropertyInfo, OptionAttribute> propBag = ReflectType<T>();
            foreach(KeyValuePair<PropertyInfo, OptionAttribute> kvp in propBag)
            {
                PropertyInfo prop = kvp.Key;
                OptionAttribute attr = kvp.Value;

                string key = string.Empty;
                if (!string.IsNullOrEmpty(kvp.Value.ShortName) && string.IsNullOrEmpty(attr.LongName))
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

                if (string.IsNullOrWhiteSpace(desc))
                {
                    optionHash.Add(key, $"{attr.FriendlyName}");
                }
                else
                {
                    optionHash.Add(key, $"{attr.FriendlyName} - {desc}");
                }
            }

            foreach (KeyValuePair<string, string> pair in optionHash)
            {
                stdout.WriteLine($"{pair.Key.PadRight(maxKeyLen, ' ')}\t| {pair.Value}");
            }

            stdout.WriteLine();
        }

        protected static Dictionary<PropertyInfo, OptionAttribute> ReflectType<T>()
        {
            Type type = typeof(T);
            if (_reflectCache.ContainsKey(type))
                return _reflectCache[type];

            Dictionary<PropertyInfo, OptionAttribute> typeCache = new Dictionary<PropertyInfo, OptionAttribute>();

            foreach (PropertyInfo prop in type.GetProperties())
            {
                foreach (OptionAttribute attr in prop.GetCustomAttributes(typeof(OptionAttribute)))
                {
                    typeCache.Add(prop, attr);
                }
            }

            _reflectCache[type] = typeCache;

            return typeCache;
        }
    }
}