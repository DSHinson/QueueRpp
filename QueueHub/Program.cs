using Queue;
using QueueHub.Server;

namespace QueueHub
{
    internal class Program
    {
        private static bool _exitRequested = false;

        static void Main(string[] args)
        {
            Console.Title = "Message Server";
            Console.WriteLine("Starting...");
            Console.WriteLine("Press Ctrl+C to exit.");

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                _exitRequested = true;
                cts.Cancel();
            };

            Task listenerTask = Task.Run(() => Hub.Listener(cts.Token));

            while (!_exitRequested)
            {
                // Keep the application alive
                Thread.Sleep(100);
            }

            // Wait for the listener task to complete
            listenerTask.Wait();

        }


    }
}
