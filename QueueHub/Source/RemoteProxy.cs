using Newtonsoft.Json;
using Queue;
using QueueHub.Source.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QueueHub.Source
{
    public class RemoteProxy<T> : DispatchProxy where T : class
    {
        private string remoteAddress;
        private int remotePort;
        public void Configure(string address, int port)
        {
            remoteAddress = address;
            remotePort = port;
        }
         protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (targetMethod.Name.ToUpper().Contains("SENDMESSAGE"))
            { 
                return handleSendMessage(targetMethod, args);
            }
            else if (targetMethod.Name.ToUpper().Contains("SUBSCRIBE"))
            { 
                return handleSendSubscribe(targetMethod, args);
            }
            else if (targetMethod.Name.ToUpper().Contains("UNSUBSCRIBE"))
            {
                return handleSendUnsubscribe(targetMethod, args);
            }

            else { 
                throw new NotImplementedException(targetMethod.Name);
            }
        }

        private object handleSendMessage(MethodInfo targetMethod, object[] args)
        {
            var methodToCall = new MethodCalldto<Message>
            {
                MethodName = targetMethod.Name,
                Args = (Message)args[0]
            };

            string serializedData = JsonConvert.SerializeObject(methodToCall);

            using (var client = new TcpClient(remoteAddress, remotePort))
            {
                byte[] sendBytes = Encoding.UTF8.GetBytes(serializedData);
                client.GetStream().Write(sendBytes, 0, sendBytes.Length);

                byte[] buffer = new byte[256];
                int bytesRead = client.GetStream().Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                return response;
            }
        }
        private object handleSendSubscribe(MethodInfo targetMethod, object[] args)
        {
            var methodToCall = new MethodCalldto<object[]>
            {
                MethodName = targetMethod.Name,
                Args = args
            };

            string serializedData = JsonConvert.SerializeObject(methodToCall);

            using (var client = new TcpClient(remoteAddress, remotePort))
            {
                byte[] sendBytes = Encoding.UTF8.GetBytes(serializedData);
                client.GetStream().Write(sendBytes, 0, sendBytes.Length);

                byte[] buffer = new byte[256];
                int bytesRead = client.GetStream().Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                return response;
            }
        }
        private object handleSendUnsubscribe(MethodInfo targetMethod, object[] args)
        {
            var methodToCall = new MethodCalldto<object[]>
            {
                MethodName = targetMethod.Name,
                Args = args
            };

            string serializedData = JsonConvert.SerializeObject(methodToCall);

            using (var client = new TcpClient(remoteAddress, remotePort))
            {
                byte[] sendBytes = Encoding.UTF8.GetBytes(serializedData);
                client.GetStream().Write(sendBytes, 0, sendBytes.Length);

                byte[] buffer = new byte[256];
                int bytesRead = client.GetStream().Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                return response;
            }
        }
    }
}
