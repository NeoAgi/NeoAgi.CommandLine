using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoAgi.CommandLine.Configuration
{
    public class OptsConfigurationSource<T> : IConfigurationSource where T : new()
    {
        public string SectionKey { get; set; }
        public string[] Arguments { get; set; }

        public OptsConfigurationSource(string section, string[] args)
        {
            SectionKey = section;
            Arguments = args;
        }

        IConfigurationProvider IConfigurationSource.Build(IConfigurationBuilder builder)
        {
            return new OptsConfigurationProvider<T>(SectionKey, Arguments);
        }
    }
}
