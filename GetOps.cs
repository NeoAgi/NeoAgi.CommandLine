using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoAgi.CommandLine
{
    public static class StringArrayExtension 
    {
        public static T GetOps<T>(this string[] args, out bool success) where T : new()
        {
            OptionManager manager = new OptionManager();
            Dictionary<string, string> dict = manager.Parse(args);

            T retVal = manager.Merge<T>(new T(), dict);

            success = true;

            return retVal;
        }
    }
}
