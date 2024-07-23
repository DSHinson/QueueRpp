using Queue;
using QueueHub.Consumer;
using QueueHub.Source.dto;

namespace ExampleMessageConsumer
{
    internal class Program
    {
        private static bool _exitRequested = false;

        static void Main(string[] args)
        {
            Console.Title = "Message Consumer";
            Console.WriteLine("Starting...");
            MessageReceiverService messageReceiver = new MessageReceiverService("127.0.0.1", 5066);
            messageReceiver.ItemReceived += (msg) =>
            {
                Console.WriteLine(msg?.Value);
            };

            Console.WriteLine("Press Ctrl+C to exit.");

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                _exitRequested = true;
            };
            while (!_exitRequested)
            {

                // Keep the application alive
                Thread.Sleep(100);
            }
        }
    }
}
