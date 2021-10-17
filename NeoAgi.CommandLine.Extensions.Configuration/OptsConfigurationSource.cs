using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoAgi.CommandLine.Extensions.Configuration
{
    /// <summary>
    /// Configuration Source Implementation of NeoAgi.CommandLine.GetOps()
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OptsConfigurationSource<T> : IConfigurationSource where T : new()
    {
        private string NamespacePrefix { get; set; }
        private string[] Arguments { get; set; }

        /// <summary>
        /// Default constructor to intake command line arguments and namespace prefix.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="namespacePrefix"></param>
        public OptsConfigurationSource(string[] args, string namespacePrefix = "")
        {
            NamespacePrefix = namespacePrefix;
            Arguments = args;
        }

        IConfigurationProvider IConfigurationSource.Build(IConfigurationBuilder builder)
        {
            return new OptsConfigurationProvider<T>(Arguments, NamespacePrefix);
        }
    }
}
