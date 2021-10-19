using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoAgi.CommandLine.Extensions.Configuration;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Extension Methods for Microsoft.Extensions.Configuration
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Injects Command Line Varaibles into the Configuration Builder according to NeoAgi.CommandLine semantics
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="args"></param>
        /// <param name="namespacePrefix">Namespace Prefix to load values into Configuration[].  Mirror this value in Configuration.GetSection() to override configuration cadence.</param>
        /// <param name="outputStream">Optional TextWriter for the CommandLine Parser to write feedback to.</param>
        /// <returns></returns>
        public static IConfigurationBuilder AddOpts<T>(this IConfigurationBuilder builder, string[] args, string namespacePrefix = "", TextWriter? outputStream = null) where T : new()
        {
            builder.Add(new OptsConfigurationSource<T>(args, namespacePrefix, outputStream));

            return builder;
        }
    }
}