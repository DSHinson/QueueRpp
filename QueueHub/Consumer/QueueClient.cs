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
using QueueHub.Server;

namespace QueueHub.Consumer
{
    /// <summary>
    /// hide away the fact that a udp client will be created here that receives messages
    /// </summary>
    public class QueueClient : IDisposable
    {
        public int Port = 5005;
        public string ServerIp = "127.0.0.1";
        public event Action<Message>? ItemReceived;
        private CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// Start the UdpClient
        /// </summary>
        public QueueClient(int port = 5005, string serverIp = "127.0.0.1")
        {
            Port = port;
            ServerIp = serverIp;
            // Start the UDP listener in a new thread
            Task.Run(() => StartListener(System.Net.IPAddress.Parse(serverIp), Port, cts.Token));
        }

        /// <summary>
        /// Sends the config to the hub so the client can receive messages
        /// </summary>
        public void Subscribe()
        {

        }

        void StartListener(IPAddress serverIp, int serverPort, CancellationToken cancellationToken)
        {

            try
            {
                using (var connectionHandler = new ConnectionHandler(serverIp.ToString(), serverPort, cancellationToken, async data =>
                {
                    Console.WriteLine("Waiting for a client to connect...");
                    var message = JsonConvert.DeserializeObject<Message>(data);
                    Console.WriteLine($"Received: {message}");
                    ItemReceived?.Invoke(message);
                }))
                {
                    connectionHandler.Start();

                    try
                    {
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            // Keep the method alive until cancellation is requested
                            Task.Delay(1000);
                        }
                    }
                    finally
                    {
                        Console.WriteLine("Listener stopped.");
                    }
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
