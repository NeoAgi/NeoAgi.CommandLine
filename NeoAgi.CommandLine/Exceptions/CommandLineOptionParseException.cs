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
        /// Construct an Exception with an Enumeration of Options Errors
        /// </summary>
        /// <param name="optionErrors"></param>
        public CommandLineOptionParseException(IEnumerable<OptionAttributeError> optionErrors) :
            this("Options Processing Contained Errors", optionErrors)
        { }

        /// <summary>
        /// Construct an Exception with an Enumeration of Options Errors
        /// </summary>
        /// <param name="message"></param>
        /// <param name="optionErrors"></param>
        public CommandLineOptionParseException(string message, IEnumerable<OptionAttributeError> optionErrors) 
            : base(message)
        {
            OptionsWithErrors.AddRange(optionErrors);
        }
    }
}
