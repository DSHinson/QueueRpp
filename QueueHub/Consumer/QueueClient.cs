using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Queue;
using QueueHub.Source.dto;
using System.Threading;

namespace QueueHub.Consumer
{
    /// <summary>
    /// hide away the fact that a udp client will be created here that receives messages
    /// </summary>
    public class QueueClient : IDisposable
    {
        public int Port = 5005;
        public string serverIp = "127.0.0.1";
        public event Action<Message>? ItemReceived;
        private CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// Start the UdpClient
        /// </summary>
        public QueueClient()
        {
            // Start the UDP listener in a new thread
            Task.Run(() => StartListener(serverIp, Port, cts.Token));
        }

        /// <summary>
        /// Sends the config to the hub so the client can receive messages
        /// </summary>
        public void Subscribe()
        {

        }

        void StartListener(string serverIp, int serverPort, CancellationToken cancellationToken)
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, Port);
                listener.Start();

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Waiting for a client to connect...");
                        using (var client = listener.AcceptTcpClient())
                        {
                            byte[] buffer = new byte[256];
                            int bytesRead = client.GetStream().Read(buffer, 0, buffer.Length);
                            string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                            Console.WriteLine($"Received: {receivedData}");
                            var message = JsonConvert.DeserializeObject<Message>(receivedData);

                            ItemReceived?.Invoke(message);
                        }
                    }
                }
                finally
                {
                    listener.Stop();
                    Console.WriteLine("Listener stopped.");
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"SocketException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        private void UnSubscribe()
        {

        }

        public void Dispose()
        {
            UnSubscribe();
            cts.Cancel();
        }


    }
}
