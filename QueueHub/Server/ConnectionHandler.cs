﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace QueueHub.Server
{
    public class ConnectionHandler:IDisposable
    {
        private TcpListener _listener;
        private CancellationToken _cancellationToken;
        private Func<string, Task>? _callback;

        public ConnectionHandler(string IPAddress,int port, CancellationToken cancellationToken, Func<string, Task> callback)
        {
            _= IPAddress ?? throw new ArgumentNullException(nameof(IPAddress));
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
            _cancellationToken = cancellationToken;

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
                        if (_callback != null)
                        {
                            _ = _callback(receivedData);
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
