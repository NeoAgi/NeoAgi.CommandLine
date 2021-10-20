using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoAgi.CommandLine
{
    /// <summary>
    /// Parse Constraint Failures
    /// </summary>
    [Flags]
    public enum OptionAttributeErrorReason
    {
        /// <summary>
        /// Required Constraint Failed
        /// </summary>
        REQUIRED = 1
    };

    /// <summary>
    /// Reason and offending OptionAttribute of a parse constraint that failed to be satisfied
    /// </summary>
    public class OptionAttributeError
    {
        /// <summary>
        /// Offending Option
        /// </summary>
        public OptionAttribute Option { get; set; }

        /// <summary>
        /// Constraint Failure Type
        /// </summary>
        public OptionAttributeErrorReason Reason { get; set; }

        /// <summary>
        /// Capture the offending OptionAttribute and Reason for the constraint failure
        /// </summary>
        /// <param name="option"></param>
        /// <param name="reason"></param>
        public OptionAttributeError(OptionAttribute option, OptionAttributeErrorReason reason)
        {
            Option = option;
            Reason = reason;
        }
    }
}
