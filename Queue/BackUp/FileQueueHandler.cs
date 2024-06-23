using Queue.Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Queue.BackUp
{
    /// <summary>
    /// This class should provide the functionality for writing and reading the file that will be the Queue persistence ,
    /// for now I will implement a text file as being the queue persistence but in the future if speed becomes an issue I will look at implementing a binary file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FileQueueHandler<T>
    {
        private readonly string filePath;
        private readonly Object fileLock;

        public FileQueueHandler()
        {
            filePath = typeof(T).Name +".txt";
            fileLock = new Object();
        }
        private void InitializeFile()
        {
            if (!System.IO.File.Exists(filePath))
            {
                using (System.IO.File.Create(filePath)) 
                { }
            }
        }

        public Queue<T> ReadFileFromQueue()
        {
            Queue<T> restore = new Queue<T>();
            lock (fileLock)
            {
                if (System.IO.File.Exists(filePath))
                {
                    var lines = System.IO.File.ReadAllLines(filePath);
                    Console.WriteLine($"Starting to restore:{lines.Length} items");
                    foreach (var line in lines)
                    {
                        var item = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(line);
                        restore.Enqueue(item ?? throw new ArgumentNullException("Queue item is null"));
                    }
                    Console.WriteLine($"restore complete");
                }
            }
            return restore;
        }

        public async Task WriteQueueToFile(T[] queue)
        {
            if (queue == null)
            {
                return;
            }
            Console.WriteLine($"Starting to backup:{queue.Length} items");
            lock (fileLock)
            {
                InitializeFile();
                using (var fileStream = new System.IO.StreamWriter(filePath, false))
                {
                    foreach (var item in queue)
                    {
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                        fileStream.WriteLine(json);
                    }
                }
            }
            Console.WriteLine($"backUp Complete");
        }

        public void TruncateLogFile(T[] queue)
        {
            lock (fileLock)
            {
                InitializeFile();
                var items = queue.ToArray();
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    foreach (var item in items)
                    {
                        string stringToWrite = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                        writer.WriteLine(stringToWrite);
                    }
                }
            }
        }


    }
}
