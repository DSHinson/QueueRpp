using Queue;
using QueueHub.Source;
using QueueHub.Source.dto;

namespace ExampleMessageSource
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Message Producer";
            var messageSendService = new MessageSenderFactory().CreateMessageSender();
            Console.WriteLine("Hello, welcome to the example event source, type your message and hit enter for it to be inserted into the queue.");
            Console.WriteLine("if you would like to quite hit Q then enter");
            string input = "";

            while (input.ToUpper() != "Q")
            {
                input = Console.ReadLine();
                messageSendService.SendMessage(new Message() { Value = input });

            }


        }
    }
}
