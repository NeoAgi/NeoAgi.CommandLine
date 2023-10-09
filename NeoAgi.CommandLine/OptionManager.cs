using System;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NeoAgi.CommandLine.Exceptions;
using System.Text.RegularExpressions;

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

        private static Regex EqualsMatch = new Regex("[-][-a-zA-Z0-9]{1,32}[=]", RegexOptions.None, new TimeSpan(0, 0, 1));

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

            // Parsing is aware of the following types of input:
            // 1. Space Delimited: '-f bar' or '--foo bar'
            // 2. Equal Delimited: '-f=bar' or '--foo=bar'
            // 3. Valueless/Flags: '-f' or '--foo'

            for (int i = 0; i < arr.Length; i++)
            {
                // Special case, if the paramater is --help, bail early and print
                if (arr[i].Equals("--help"))
                    throw new RaiseHelpException();

                // Look to see if this string starts with a -, and has a value after it
                if (arr[i].StartsWith('-'))
                {
                    // Process = delimited options  (type 2)
                    Match match = EqualsMatch.Match(arr[i]);
                    if (match.Success)
                    {
                        // At this point match contains the value including the trailing =
                        tuples.Add(match.Value.Substring(0, match.Value.Length - 1), arr[i].Substring(match.Value.Length));
                    }
                    // Process a flag if received as the final parameter  (type 3)
                    else if (arr.Length == i + 1)
                    {
                        tuples.Add(arr[i], "true");
                    }
                    // Process space delimted options as they require look aheads, this case should be last as it's inherently greedy
                    else if (arr.Length >= i + 1)
                    {
                        // Look to see if this is a Valueless flag in the middle of the argument list  (type 3)
                        if (arr[i + 1].StartsWith('-'))
                        {
                            tuples.Add(arr[i], "true");
                            continue;
                        }

                        // Otherwise, process this as a ' ' delimited option (type 1)
                        tuples.Add(arr[i], arr[i + 1]);
                        i++; // Skip processing the next argument as we know it was a value

                    }
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
                    prop.SetValue(ret, ParseValueFromArgument(prop, attr, values[$"-{attr.ShortName}"]));
                }

                if (values.ContainsKey($"--{attr.LongName}"))
                {
                    propFound = true;
                    prop.SetValue(ret, ParseValueFromArgument(prop, attr, values[$"--{attr.LongName}"]));
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
            List<OptionAttributeError> errors = new List<OptionAttributeError>();
            Dictionary<PropertyInfo, OptionAttribute> propBag = ReflectType<T>();
            Dictionary<string, string?> retVal = new Dictionary<string, string?>();

            foreach (KeyValuePair<PropertyInfo, OptionAttribute> kvp in propBag)
            {
                bool propFound = false;
                PropertyInfo prop = kvp.Key;
                OptionAttribute attr = kvp.Value;
                object? oValue = default;

                // Note: This is a condensed version of value look ups found in T Merge<T>() above
                if (values.ContainsKey($"-{attr.ShortName}"))
                {
                    propFound = true;
                    oValue = values[$"-{attr.ShortName}"];
                }

                if (values.ContainsKey($"--{attr.LongName}"))
                {
                    propFound = true;
                    oValue = values[$"--{attr.LongName}"];
                }

                if (propFound)
                {
                    string? sValue;
                    if (oValue == null)
                    {
                        sValue = null;
                    }
                    else
                    {
                        sValue = oValue.ToString();
                    }

                    retVal.Add(keyPrefix + kvp.Key.Name, sValue);
                }
                else if (!propFound && attr.Required)
                    errors.Add(new OptionAttributeError(attr, OptionAttributeErrorReason.REQUIRED));
            }

            if (errors.Count > 0)
                throw new CommandLineOptionParseException(errors);

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
                    if (!string.IsNullOrEmpty(error.Option.ShortName))
                        optionNames.Add("-" + error.Option.ShortName);

                    if (!string.IsNullOrEmpty(error.Option.LongName))
                        optionNames.Add("--" + error.Option.LongName);

                    if (error.Reason.HasFlag(OptionAttributeErrorReason.REQUIRED))
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
            foreach (KeyValuePair<PropertyInfo, OptionAttribute> kvp in propBag)
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
        /// Quick Reflector Helper to box variables to the POCO
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="attr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected static object? ParseValueFromArgument(PropertyInfo propertyInfo, OptionAttribute attr, string value)
        {
            if(propertyInfo.PropertyType == typeof(bool))
                return bool.Parse(value);

            if (propertyInfo.PropertyType == typeof(int))
                return int.Parse(value);

            if (propertyInfo.PropertyType == typeof(long))
                return long.Parse(value);

            if (propertyInfo.PropertyType == typeof(decimal))
                return decimal.Parse(value);

            // Return the string
            return value;
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
