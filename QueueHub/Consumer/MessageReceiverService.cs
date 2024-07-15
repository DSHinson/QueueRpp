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

        public MessageReceiverService()
        {
            subscribeUtil = new MessageSubscribeFactory().CreateMessageSubscriber();
            subscribeUtil.Subscribe(2222,"127.0.0.1");

            queueManager = new QueueClient();
            queueManager.ItemReceived += OnItemReceived;
        }

        private void OnItemReceived(Message msg)
        {
            ItemReceived?.Invoke(msg);
        }

        public void Dispose()
        {
            subscribeUtil.UnSubscribe(2222, "127.0.0.1");
            queueManager.ItemReceived -= OnItemReceived;
            queueManager.Dispose();
        }
    }
}
