using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueHub.Source.dto
{
    public class NetworkMessageWrapper
    {
        public MessageType Type { get; set; }
        public string Payload { get; set; }

    }
}
