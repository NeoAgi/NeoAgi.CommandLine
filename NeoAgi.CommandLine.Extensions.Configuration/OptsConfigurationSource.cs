using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
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
        private TextWriter? OutputStream { get; set; } = null;
        private string NamespacePrefix { get; set; } = string.Empty;
        private string[] Arguments { get; set; }

        /// <summary>
        /// Default constructor to intake command line arguments and namespace prefix.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="namespacePrefix"></param>
        /// <param name="outputStream"></param>
        public OptsConfigurationSource(string[] args, string namespacePrefix = "", TextWriter? outputStream = null)
        {
            Arguments = args;
            NamespacePrefix = namespacePrefix;
            OutputStream = outputStream;
        }

        IConfigurationProvider IConfigurationSource.Build(IConfigurationBuilder builder)
        {
            return new OptsConfigurationProvider<T>(Arguments, NamespacePrefix, OutputStream);
        }
    }
}
