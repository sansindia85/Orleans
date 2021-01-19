using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiloHost
{
    public class GrainInformation
    {
        public GrainInformation()
        {
            Methods = new List<string>();
        }
        public List<string> Methods { get; set; }
    }
}
