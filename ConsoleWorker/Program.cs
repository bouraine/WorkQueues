using System;
using System.Threading.Tasks;
using EasyNetQ;
using Messages;

namespace Worker1
{
    class Program
    {
        static void Main(string[] args)
        {
            // prefetchcount=15
            using (var bus = RabbitHutch.CreateBus("host=localhost;username=user;password=password"))
            {
                bus.Subscribe<TextMessage>("test", HandleTextMessage);

                // bus.SubscribeAsync<TextMessage>("test",
                // message => Task.Factory.StartNew(() => HandleTextMessage(message)));

                Console.WriteLine("Listening for messages. Hit <return> to quit.");
                Console.ReadLine();
            }
        }


        static async void HandleTextMessage(TextMessage textMessage)
        {
            await Task.Delay(5000);


            Console.WriteLine("Got message: {0}", textMessage.Text);
            Console.ResetColor();
        }
    }
}