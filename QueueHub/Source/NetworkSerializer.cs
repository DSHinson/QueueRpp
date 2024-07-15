using Newtonsoft.Json;
using QueueHub.Source.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueHub.Source
{
    [Flags]
    public enum MessageType
    {
        MethodCall = 1 << 0,
        Message = 1 << 1,
    }
    public static class  NetworkSerializer
    {
        public static byte[] serializeToJsonToBytes(object obj)
        {
            NetworkMessageWrapper wrapper = new NetworkMessageWrapper();
            wrapper.Payload = JsonConvert.SerializeObject(obj);
            Type objType = obj.GetType();

            if (objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(MethodCalldto<>))
            {
                wrapper.Type = MessageType.MethodCall;
            }
            else if (objType == typeof(Message))
            {
                wrapper.Type = MessageType.Message;
            }
            else
            {
                throw new InvalidOperationException("Unsupported object type");
            }

            byte[] sendBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(wrapper));
            return sendBytes;
        }
    }
}
