using Queue;
using QueueHub.Source;
using QueueHub.Source.dto;

namespace QueueHub.Consumer
{
    public class MessageReceiverService
    {
        private readonly QueueClient queueManager;
        private readonly ISubscribeUtil subscribeUtil;

        public event Action<Message> ItemReceived;

        public MessageReceiverService(string IpAddress , int port)
        {
            subscribeUtil = new MessageSubscribeFactory().CreateMessageSubscriber();
            subscribeUtil.Subscribe(port, IpAddress);
            
            queueManager = new QueueClient(IpAddress, port);
            queueManager.ItemReceived += OnItemReceived;
        }

        private void OnItemReceived(Message msg)
        {
            ItemReceived?.Invoke(msg);
        }

        public void Dispose()
        {
            subscribeUtil.UnSubscribe(5005, "127.0.0.1");
            queueManager.ItemReceived -= OnItemReceived;
            queueManager.Dispose();
        }
    }
}
