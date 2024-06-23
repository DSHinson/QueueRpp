using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queue.Queue
{
    /// <summary>
    /// As the name suggests this class provides interactions for the in memory queue of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InMemoryQueue<T>
    {
        /// <summary>
        /// There is no interface provided for this class as I dont feel that there will be a need to change the implementation of the in memory queue functionality
        /// </summary>
        private Queue<T> _queue;
        private readonly SemaphoreSlim _semaphore;

        public InMemoryQueue()
        {
            _queue = new Queue<T>();
            _semaphore = new SemaphoreSlim(1,1);
        }
        public async void Enqueue(T message)
        {
            try { 
                await _semaphore.WaitAsync();
                _queue.Enqueue(message);
            }
            catch { }
            finally
            {
                _semaphore.Release();
            }
        }
        public async Task<T> DequeueAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!_queue.TryDequeue(out T message))
                {
                    //swallow the no item dequeue error
                }
                return message;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public void SetQueue(Queue<T> newQ)
        { 
            _queue = newQ;
        }
        public T[] ToArray()
        {
            return _queue.ToArray();
        }
        public int Count()
        {
            return _queue.Count;
        }
    }
}
