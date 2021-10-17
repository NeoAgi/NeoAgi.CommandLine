using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoAgi.CommandLine;
using NeoAgi.CommandLine.Extensions.Configuration;

namespace NeoAgi.CommandLine.Extensions.Configuration
{
    /// <summary>
    /// Configuration Provider Implemetnation for NeoAgi.CommandLine.GetOps()
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OptsConfigurationProvider<T> : ConfigurationProvider where T : new()
    {
        private const char SECTION_SEPARATOR = ':';

        private string NamespacePrefix { get; set; }
        private string[] Arguments { get; set; }

        /// <summary>
        /// Default constructor to intake command line arguments and namespace prefix.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="namespacePrefix"></param>
        public OptsConfigurationProvider(string[] args, string namespacePrefix = "")
        {
            NamespacePrefix = namespacePrefix;
            Arguments = args;
        }

        /// <summary>
        /// Injects NeoAgi.CommandLine.GetOps into the Configuration symbol table
        /// </summary>
        public override void Load()
        {
            if (!string.IsNullOrEmpty(NamespacePrefix) && !NamespacePrefix.EndsWith(SECTION_SEPARATOR))
                NamespacePrefix += SECTION_SEPARATOR;

            Data = Arguments.FlattenOpts<T>(NamespacePrefix);
        }
    }
}
