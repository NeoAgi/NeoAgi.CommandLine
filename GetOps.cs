using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoAgi.CommandLine
{
    public static class StringArrayExtension 
    {
        public static T GetOps<T>(this string[] args) where T : new()
        {
            return new T();
        }
    }
}
