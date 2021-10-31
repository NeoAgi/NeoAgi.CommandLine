using System;
using NeoAgi.CommandLine;
using NeoAgi.CommandLine.Exceptions;
using System.Collections.Generic;

namespace NeoAgi.CommandLine.Exceptions
{
    /// <summary>
    /// Raised when an option is marked required yet not satisified with the aruments provided
    /// </summary>
    public class CommandLineOptionParseException : ApplicationException
    {
        /// <summary>
        /// Option that failed to be satisfied.
        /// </summary>
        public List<OptionAttributeError> OptionsWithErrors { get; set; } = new List<OptionAttributeError>();

        /// <summary>
        /// Required Constructor that accepts the offending OptionAttributeError
        /// </summary>
        /// <param name="optionErrors"></param>
        public CommandLineOptionParseException(params OptionAttributeError[] optionErrors)
        {
             OptionsWithErrors.AddRange(optionErrors);
        }

        /// <summary>
        /// Helper to accept a type of List&lt;OptionAttributeError&gt; to avoid .ToArray() when needed.
        /// </summary>
        /// <param name="optionErrors"></param>
        public CommandLineOptionParseException(List<OptionAttributeError> optionErrors)
        {
            OptionsWithErrors.AddRange(optionErrors.ToArray());
        }
    }
}
