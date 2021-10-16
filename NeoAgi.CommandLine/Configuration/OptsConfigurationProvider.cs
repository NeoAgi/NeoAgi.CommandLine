using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoAgi.CommandLine;

namespace NeoAgi.CommandLine.Configuration
{
    public class OptsConfigurationProvider<T> : ConfigurationProvider where T : new()
    {
        public string SectionKey { get; set; }
        public string[] Arguments { get; set; }

        public OptsConfigurationProvider(string section, string[] args)
        {
            SectionKey = section;
            Arguments = args;
        }

        public override void Load()
        {
            if(!string.IsNullOrEmpty(SectionKey))
            {
                SectionKey = SectionKey + ":";
            }

            Data = Arguments.FlattenOpts<T>(SectionKey);
        }
    }
}
