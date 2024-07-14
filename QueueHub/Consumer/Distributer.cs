using Queue;
using QueueHub.Source.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace QueueHub.Consumer
{
    public class Distributer : IDisposable
    {
        private List<string> clients = new List<string>();
        // subscribe to the event
        public void Subscribe(MethodCalldto<object[]> subscibeInfo)
        {
            clients.Add(subscibeInfo.MethodName);
        }

        // unsubscribe to the event
        public void Unsubscribe(MethodCalldto<object[]> unsubscibeInfo)
        {
            clients.Remove(unsubscibeInfo.MethodName);
        }

        public void sendMessageToConsumer(Message message) {
            Console.WriteLine($"Sending message:{message?.Value}");

            using (var client = new TcpClient("127.0.0.1", 5005))
            {
                string serializedData = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                byte[] sendBytes = Encoding.UTF8.GetBytes(serializedData);
                client.GetStream().Write(sendBytes, 0, sendBytes.Length);

                byte[] buffer = new byte[256];
                int bytesRead = client.GetStream().Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
        }

        public void Dispose()
        {
            ///kill all the client connections
        }
    }
}
