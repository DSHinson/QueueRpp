using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Queue;
using QueueHub.Source.dto;
using System.Reflection;
using QueueHub.Consumer;

namespace QueueHub.Server
{
    public static class Hub
    {
        private static readonly int Port = 11000;

        public static async void Listener(CancellationToken cancellationToken)
        {
            using (var distributer = new Distributer())
            using (var _Q = new QInteractions<Message>())
            using (var connectionHandler = new ConnectionHandler(IPAddress.Any.ToString(),Port, cancellationToken, async data =>
            {
                var methodCall = JsonConvert.DeserializeObject<MethodCalldto<Message>>(data);
                _Q.Enqueue(methodCall?.Args);
            }))
            {
                _Q.ItemDequeued.Event += distributer.sendMessageToConsumer;
                
                connectionHandler.Start();

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        // Keep the method alive until cancellation is requested
                        await Task.Delay(1000);
                    }
                }
                finally
                {
                    Console.WriteLine("Listener stopped.");
                }
            }
        }



    }
}
