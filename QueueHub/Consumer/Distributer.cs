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
            DistributerTCPClient client = new DistributerTCPClient(
                subscibeInfo.Args[1].ToString()
              , int.Parse(subscibeInfo.Args[0].ToString()));
            clients.Add(client);
        }

        // unsubscribe to the event
        public void Unsubscribe(MethodCalldto<object[]> unsubscibeInfo)
        {
            DistributerTCPClient client = clients.Where(x=>x.Id == unsubscibeInfo.Args[0].ToString()).First();
            client.Dispose();
            clients.Remove(client);
        }

        public async void sendMessageToConsumer(Message message) {
            Console.WriteLine($"Sending message: {message?.Value}");

            var tasks = clients.Select(subscriber => Task.Run(() => subscriber.Send(message))).ToList();

            await Task.WhenAll(tasks);
        }

        public void Dispose()
        {
            
        }
    }
}
