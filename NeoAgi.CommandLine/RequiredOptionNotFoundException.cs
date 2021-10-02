using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoAgi.CommandLine
{
    public class RequiredOptionNotFoundException : ApplicationException
    {
        public OptionAttribute Option { get; set; }

        public RequiredOptionNotFoundException(OptionAttribute option)
        {
            Option = option;
        }
    }
}
