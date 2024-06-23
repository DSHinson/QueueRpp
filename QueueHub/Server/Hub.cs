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

        public static void Listener(CancellationToken cancellationToken)
        {   using (var distributer = new Distributer())
            using (var _Q = new QInteractions<Message>())
            {
                
                TcpListener listener = new TcpListener(IPAddress.Any, Port);
                listener.Start();

                try
                {
                    _Q.ItemDequeued.Event += distributer.sendMessageToConsumer;
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Waiting for a client to connect...");
                        using (var client = listener.AcceptTcpClient())
                        {
                            byte[] buffer = new byte[256];
                            int bytesRead = client.GetStream().Read(buffer, 0, buffer.Length);
                            string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                            Console.WriteLine($"Received: {receivedData}");
                            var methodCall = JsonConvert.DeserializeObject<MethodCalldto<Message>>(receivedData);

                            Type type = typeof(QInteractions<Message>);
                            _Q.Enqueue(methodCall?.Args);

                        }
                    }
                }
                finally
                {
                    listener.Stop();
                    Console.WriteLine("Listener stopped.");
                }
            }
        }
    }
}
