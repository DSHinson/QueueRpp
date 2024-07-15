using Queue;
using QueueHub.Source;
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
        private List<KeyValuePair<string,TcpClient>> clients =new List<KeyValuePair<string, TcpClient>>();
        // subscribe to the event
        public void Subscribe(MethodCalldto<object[]> subscibeInfo)
        {
            TcpClient client = new TcpClient(subscibeInfo.Args[1].ToString(), int.Parse(subscibeInfo.Args[0].ToString()));
            clients.Insert(0,new KeyValuePair<string, TcpClient>(subscibeInfo.Args[0].ToString(), client));
        }

        // unsubscribe to the event
        public void Unsubscribe(MethodCalldto<object[]> unsubscibeInfo)
        {
            KeyValuePair<string, TcpClient> client = clients.Where(x=>x.Key == unsubscibeInfo.Args[0].ToString()).First();
            clients.Remove(client);
            client.Value.Close();
            client.Value.Dispose();
        }

        public void sendMessageToConsumer(Message message) {
            Console.WriteLine($"Sending message:{message?.Value}");

            foreach (var kvp in clients)
            { 
                TcpClient client = kvp.Value;
                byte[] sendBytes = NetworkSerializer.serializeToJsonToBytes(message);
                client.GetStream().Write(sendBytes, 0, sendBytes.Length);

                byte[] buffer = new byte[256];
                int bytesRead = client.GetStream().Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
        }

        public void Dispose()
        {
            ///kill all the client connections
            foreach (var client in clients) {
                client.Value.Close();
                client.Value.Dispose();
            }
        }
    }
}
