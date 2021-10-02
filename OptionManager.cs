using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace NeoAgi.CommandLine
{
    internal class OptionManager
    {
        public OptionManager() { }

        public Dictionary<string, string> Parse(string[] arr)
        {
            Dictionary<string, string> tuples = new Dictionary<string, string>();

            for(int i = 0; i < arr.Length; i++)
            {
                // Look to see if this string starts with a -, and has a value after it
                if(arr[i].StartsWith('-') && arr.Length >= i + 1)
                {
                    tuples.Add(arr[i], arr[i + 1]);
                    i++;
                }
            }

            return tuples;
        }

        public T Merge<T>(T ret, Dictionary<string, string> values)
        {
            Type typeT = typeof(T);
            foreach(PropertyInfo prop in typeT.GetProperties())
            {
                foreach(OptionAttribute attr in prop.GetCustomAttributes(typeof(OptionAttribute)))
                {
                    if(values.ContainsKey($"-{attr.ShortName}"))
                    {
                        prop.SetValue(ret, values[$"-{attr.ShortName}"]);
                    }

                    if(values.ContainsKey($"--{attr.LongName}"))
                    {
                        prop.SetValue(ret, values[$"--{attr.LongName}"]);
                    }
                }
            }

            return ret;
        }
    }
}
