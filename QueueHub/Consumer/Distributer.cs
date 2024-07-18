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
        private List<DistributerTCPClient> clients =new List<DistributerTCPClient>();
        // subscribe to the event
        public void Subscribe(MethodCalldto<object[]> subscibeInfo)
        {
            subscibeInfo.Args[1].ToString();
                int.Parse(subscibeInfo.Args[0].ToString();
            DistributerTCPClient client = new DistributerTCPClient();
            clients.Add(client);
        }

        // unsubscribe to the event
        public void Unsubscribe(MethodCalldto<object[]> unsubscibeInfo)
        {
            DistributerTCPClient client = clients.Where(x=>x.Id == unsubscibeInfo.Args[0].ToString()).First();
            clients.Remove(client);
            client.client.Close();
            client.client.Dispose();
        }

        public void sendMessageToConsumer(Message message) {
            Console.WriteLine($"Sending message:{message?.Value}");

            foreach (var subscriber in clients)
            {
                subscriber.Send(message);      
            }
        }

        public void Dispose()
        {
            
        }
    }
}
