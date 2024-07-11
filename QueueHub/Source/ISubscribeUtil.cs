using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueHub.Source
{
    public interface ISubscribeUtil
    {
        public void Subscribe(int port,string IPAddress);

        public void UnSubscribe(int port, string IPAddress);
    }
}
