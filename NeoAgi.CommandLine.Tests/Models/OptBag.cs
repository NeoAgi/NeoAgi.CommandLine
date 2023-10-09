using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoAgi.CommandLine;

namespace NeoAgi.CommandLine.Tests.Models
{
    internal class OptBag
    {
        [Option(ShortName = "l", LongName = "location", FriendlyName = "Location", Description = "Something Longer", Required = true)]
        public string Location { get; set; } = string.Empty;
        [Option(ShortName = "o", LongName = "option", FriendlyName = "Option", Description = "A longer description", Required = true)]
        public string Opt { get; set; } = string.Empty;


        [Option(LongName = "dry-run", FriendlyName = "Dry Run", Description = "Parameter used to test for valueless flags", Required = false)]
        public bool DryRun { get; set; } = false;

        [Option(ShortName = "v", FriendlyName = "Verbosity", Description = "Parameter used to test for valueless flags", Required = false)]
        public bool Verbosity { get; set; } = false;
    }
}
