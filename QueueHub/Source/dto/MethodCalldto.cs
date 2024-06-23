using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueHub.Source.dto
{
    public class MethodCalldto<T>
    {
        public string MethodName { get; set; }
        public T Args { get; set; }
    }
}
