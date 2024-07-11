using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QueueHub.Source
{
    public class MessageSubscribeFactory
    {
        public ISubscribeUtil CreateMessageSubscriber()
        {
            var proxy = DispatchProxy.Create<ISubscribeUtil, RemoteProxy<ISubscribeUtil>>();
            ((RemoteProxy<ISubscribeUtil>)proxy).Configure("127.0.0.1", 11000);
            return proxy;
        }
    }
}
