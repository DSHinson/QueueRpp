using Newtonsoft.Json;
using Queue;
using QueueHub.Consumer;
using QueueHub.Source;
using QueueHub.Source.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QueueHub.Server
{
    public class ConnectionHandler:IDisposable
    {
        private class ClientWrapper()
        {   public string Id 
            { 
                get
                {
                    return _address.Address.ToString();
                } 
            }
            public TcpClient client
            {
                get
                {
                    return _client;
                }
            }
            private TcpClient _client;
            private int _port;
            private IPAddress _address;
        }

        private TcpListener _listener;
        private CancellationToken _cancellationToken;
        private Func<string, Task>? _callback;
        private readonly Distributer? distributer;

        public ConnectionHandler( string IPAddress,int port, CancellationToken cancellationToken, Func<string, Task> callback, Distributer? distrub = null)
        {
            _= IPAddress ?? throw new ArgumentNullException(nameof(IPAddress));
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
            _cancellationToken = cancellationToken;
            distributer = distrub;

            _listener = new TcpListener(System.Net.IPAddress.Parse(IPAddress), port);
        }

        public void Dispose()
        {
            _listener.Stop();
            _listener.Dispose();
        }

        public void Start()
        { 
            _listener.Start();
            Task.Run(() => { AcceptConnections(_cancellationToken); });
        }

        private async Task AcceptConnections(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Waiting for a client to connect...");
                    using (var client = await _listener.AcceptTcpClientAsync())
                    {
                        byte[] buffer = new byte[256];
                        int bytesRead = await client.GetStream().ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Received: {receivedData}");

                        var networkPacket = JsonConvert.DeserializeObject<NetworkMessageWrapper>(receivedData);

                        if (networkPacket != null && networkPacket.Type == MessageType.Message)
                        {
                            if (_callback != null)
                            {
                                _ = _callback(networkPacket.Payload);
                            }
                        }
                        if (networkPacket != null && networkPacket.Type == MessageType.MethodCall)
                        {
                            var methodCall = JsonConvert.DeserializeObject<MethodCalldto<object[]>>(networkPacket.Payload);

                            if (methodCall.MethodName.ToUpper().Contains("UNSUBSCRIBE"))
                            {
                                distributer?.Unsubscribe(methodCall);
                                
                            }
                            else if (methodCall.MethodName.ToUpper().Contains("SUBSCRIBE"))
                            {
                                distributer?.Subscribe(methodCall);
                            }
                            
                            
                        }
                       

                        
                        
                    }
                }
            }
            catch (SocketException se) 
            {
                Console.WriteLine(se.ToString());
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }
    }
}
