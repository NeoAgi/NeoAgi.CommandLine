using System;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NeoAgi.CommandLine.Exceptions;

namespace NeoAgi.CommandLine
{
    /// <summary>
    /// Manager of Options to Parse, Merge, and Render Information about the process.
    /// </summary>
    public class OptionManager
    {
        /// <summary>
        /// List of Errors Encountered.  If > 0 the parse can be consiered unhealthy.
        /// </summary>
        public List<OptionAttributeError> Errors { get; set; } = new List<OptionAttributeError>();

        private static Dictionary<Type, Dictionary<PropertyInfo, OptionAttribute>> _reflectCache = new Dictionary<Type, Dictionary<PropertyInfo, OptionAttribute>>();

        /// <summary>
        /// Default Constructor
        /// </summary>
        public OptionManager() { }

        /// <summary>
        /// Method to parse the inputted string[] and return corresponding tuples
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        /// <exception cref="RaiseHelpException"></exception>
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

        /// <summary>
        /// Validator of OptionAttributes on T with Parsed parameters from Parse()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ret"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        /// <exception cref="CommandLineOptionParseException"></exception>
        public static T Merge<T>(T ret, Dictionary<string, string> values)
        {
            List<OptionAttributeError> errors = new List<OptionAttributeError>();
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
                    errors.Add(new OptionAttributeError(attr, OptionAttributeErrorReason.REQUIRED));
            }

            if (errors.Count > 0)
                throw new CommandLineOptionParseException(errors);

            return ret;
        }

        /// <summary>
        /// Validator of OptionsAttributes on T flattened to a Dictionary&lt;string, string&gt;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ret"></param>
        /// <param name="keyPrefix"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        /// <exception cref="CommandLineOptionParseException"></exception>
        public Dictionary<string, string?> Flatten<T>(T ret, string keyPrefix, Dictionary<string, string> values)
        {
            ret = Merge<T>(ret, values);

            Dictionary<PropertyInfo, OptionAttribute> propBag = ReflectType<T>();
            Dictionary<string, string?> retVal = new Dictionary<string, string?>();

            foreach(KeyValuePair<PropertyInfo, OptionAttribute> kvp in propBag)
            {
                string? sValue;
                object? oValue = kvp.Key.GetValue(ret);
                if(oValue == null)
                {
                    sValue = null;
                } else
                {
                    sValue = oValue.ToString();
                }

                retVal.Add(keyPrefix + kvp.Key.Name, sValue);
            }

            return retVal;
        }

        /// <summary>
        /// Default Print Template
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stdout"></param>
        /// <param name="errors"></param>
        public void PrintHelp<T>(TextWriter stdout, IEnumerable<OptionAttributeError>? errors = null) where T : new()
        {
            PrintHelpHeader(stdout);
            if (errors != null) PrintHelpErrors(stdout, errors);
            PrintHelpOptions<T>(stdout);
        }

        /// <summary>
        /// Default Header Template Component
        /// </summary>
        /// <param name="stdout"></param>
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

        /// <summary>
        /// Default Error Template Component
        /// </summary>
        /// <param name="output"></param>
        /// <param name="errors"></param>
        public static void PrintHelpErrors(TextWriter output, IEnumerable<OptionAttributeError> errors)
        {
            if (errors.Count() > 0)
            {
                output.WriteLine("ERROR(s):");
                foreach (OptionAttributeError error in errors)
                {
                    List<string> optionNames = new List<string>(2);
                    if(!string.IsNullOrEmpty(error.Option.ShortName))
                        optionNames.Add("-" + error.Option.ShortName);

                    if (!string.IsNullOrEmpty(error.Option.LongName))
                        optionNames.Add("--" + error.Option.LongName);

                    if(error.Reason.HasFlag(OptionAttributeErrorReason.REQUIRED))
                        output.WriteLine($"\tThe required option '{error.Option.FriendlyName}' was not provided by {string.Join(" or ", optionNames.ToArray())}");
                }
                output.WriteLine();
            }
        }

        /// <summary>
        /// Default Option Summary Template Component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stdout"></param>
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

        /// <summary>
        /// Helper function to cache the reflection of properties containing OptionAttributes.
        /// </summary>
        /// <remarks>This method may be better located in a satelite assembly.</remarks>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
