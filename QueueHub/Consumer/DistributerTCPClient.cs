using System.Net.Sockets;
using System.Net;
using System.Text;
using QueueHub.Source;
using System.Collections.Concurrent;

namespace QueueHub.Consumer
{
    public class DistributerTCPClient : IDisposable
    {
        private static readonly ConcurrentDictionary<string, TcpClient> _clientCache = new ConcurrentDictionary<string, TcpClient>();
        private readonly IPAddress _address;
        private readonly int _port;
        private TcpClient _client;
        private readonly string _cacheKey;

        public DistributerTCPClient(string address, int port)
        {
            _address = IPAddress.Parse(address ?? throw new ArgumentNullException(nameof(address)));
            _port = port > 0 ? port : throw new ArgumentOutOfRangeException(nameof(port));
            _cacheKey = $"{_address}:{_port}";
            _client = new TcpClient();
        }

        /// <summary>
        /// Gets the ID of the client, which is the string representation of the IP address.
        /// </summary>
        public string Id => _address.ToString();

        /// <summary>
        /// Gets the underlying TcpClient instance.
        /// </summary>
        public TcpClient Client => _client;

        /// <summary>
        /// Sends a packet to the server and returns the response as a string.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        /// <returns>The response from the server.</returns>
        public string Send(object packet)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            byte[] sendBytes = NetworkSerializer.serializeToJsonToBytes(packet);

            Open();

            NetworkStream stream = _client.GetStream();
            stream.Write(sendBytes, 0, sendBytes.Length);

            byte[] buffer = new byte[256];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            Close();

            return response;
        }

        /// <summary>
        /// Opens the connection to the server.
        /// </summary>
        private void Open()
        {
            if (!_clientCache.TryGetValue(_cacheKey, out _client))
            {
                try
                {
                    _client = new TcpClient();
                    _client.Connect(_address, _port);
                    _clientCache.TryAdd(_cacheKey, _client);
                }
                catch (Exception err){
                    Console.WriteLine(err.ToString());
                }
            }
        }

        /// <summary>
        /// Closes the connection to the server and adds the TcpClient back to the cache if still connected.
        /// </summary>
        private void Close()
        {
            if (_client.Connected)
            {
                // Close the network stream and the client connection
                _client.GetStream().Close();
                _client.Close();

                // Add the TcpClient back to the cache
                _clientCache[_cacheKey] = _client;
            }
            else
            {
                // Remove the TcpClient from the cache if it's no longer connected
                _clientCache.TryRemove(_cacheKey, out _);
                _client.Dispose();
            }
        }

        /// <summary>
        /// Disposes the TcpClient instance.
        /// </summary>
        public void Dispose()
        {
            if (_clientCache.TryRemove(_cacheKey, out TcpClient cachedClient))
            {
                cachedClient.Dispose();
            }
            _client = null;
        }
    }
}
