using Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueHub.Source
{
    public interface ISendMessage
    {
       public void SendMessage(Message msg);
    }
}
