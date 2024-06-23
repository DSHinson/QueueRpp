using Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QueueHub.Source
{
    public class MessageSenderFactory
    {
        public ISendMessage CreateMessageSender()
        {
            var proxy = DispatchProxy.Create<ISendMessage, RemoteProxy<ISendMessage>>();
            ((RemoteProxy<ISendMessage>)proxy).Configure("127.0.0.1", 11000);
            return proxy;
        }
    }
}
