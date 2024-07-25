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
        public int Port;
        public string ServerIp;
        public event Action<Message>? ItemReceived;
        private CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// Start the UdpClient
        /// </summary>
        public QueueClient(string serverIp,int port)
        {
            Port = port;
            ServerIp = serverIp ?? throw new ArgumentNullException(nameof(serverIp));

            // Start the UDP listener in a new thread
            Task.Run(() => StartListener(System.Net.IPAddress.Parse(serverIp), Port, cts.Token));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="serverPort"></param>
        /// <param name="cancellationToken"></param>
        private async void StartListener(IPAddress serverIp, int serverPort, CancellationToken cancellationToken)
        {
            try
            {
                using (var connectionHandler = new ConnectionHandler(serverIp.ToString(), serverPort, cancellationToken, async data =>
                {
                    Console.WriteLine("Waiting for a client to connect...");
                    var message = JsonConvert.DeserializeObject<Message>(data);
                    ItemReceived?.Invoke(message);
                }))
                {
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
            catch (SocketException ex)
            {
                Console.WriteLine($"SocketException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public void Dispose()
        {
            cts.Cancel();
        }


    }
}
