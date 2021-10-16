using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoAgi.CommandLine.Configuration;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddOpts<T>(this IConfigurationBuilder builder, string section, string[] args) where T : new()
        {
            builder.Add(new OptsConfigurationSource<T>(section, args));

            return builder;
        }
    }
}