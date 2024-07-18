using Queue.BackUp;
using Queue.Queue;

namespace Queue
{
    /// <summary>
    /// This class is used to insert items at the bottom of the queue and remove from the top
    /// </summary>
    public class QInteractions<T> : IDisposable
    {
        protected FileQueueHandler<T> fileQueueHandler;
        protected InMemoryQueue<T> inMemoryQueue; 
        public Utils.EventHandler<T> ItemDequeued = new();
        private Task dequeueTask;
        private CancellationTokenSource dequeueCancellationTokenSource;

        private Task backUpTask;
        private CancellationTokenSource backUpTaskCancellationTokenSource;
        public QInteractions()
        {
            // try and build queue from a back-up 
            fileQueueHandler = new FileQueueHandler<T>();
            inMemoryQueue = new InMemoryQueue<T>();

            inMemoryQueue.SetQueue(fileQueueHandler.ReadFileFromQueue()?? new Queue<T>());

            Task.Delay(TimeSpan.FromMilliseconds(1000));
            // Start the dequeue task
            dequeueCancellationTokenSource = new CancellationTokenSource();
            dequeueTask = Task.Run(() => DequeueContinuously(dequeueCancellationTokenSource.Token));

            backUpTaskCancellationTokenSource = new CancellationTokenSource();
            backUpTask = Task.Run(() => { SaveQueueToFile(backUpTaskCancellationTokenSource.Token); });

        }
        

        public void Enqueue(T value)
        {
            inMemoryQueue.Enqueue(value);
        }

        public async Task<T> DequeueAsync()
        {
          return await inMemoryQueue.DequeueAsync();
        }

        private async Task DequeueContinuously(CancellationToken cancellationToken)
        {
            
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("dequeue poll..");
                try
                {
                    // Poll for items and process them
                   var result = await DequeueAsync();
                    if (result != null)
                    {
                        Console.WriteLine("Item dequeued");
                        ItemDequeued?.Raise(result);
                    }
                    // Wait for a short interval to avoid tight loop if queue is empty
                    await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // Task was cancelled, exit the loop
                    break;
                }
            }
        }

        private async Task SaveQueueToFile(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Poll for items and process them
                    await fileQueueHandler.WriteQueueToFile(inMemoryQueue.ToArray());

                    // Wait for a short interval to avoid tight loop if queue is empty
                    await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // Task was cancelled, exit the loop
                    break;
                }
            }
        }


        public void Dispose()
        {
            dequeueCancellationTokenSource.Cancel();
            dequeueCancellationTokenSource.Dispose();
            dequeueTask.Dispose();

            backUpTaskCancellationTokenSource.Cancel();
            backUpTaskCancellationTokenSource.Dispose();
            backUpTask.Dispose();
        }
    }
}
