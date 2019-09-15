using System;
using EasyNetQ;
using Messages;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConnectionConfiguration
            {
                UserName = "user",
                Password = "password"
            };

            using (var bus = RabbitHutch.CreateBus("host=localhost;username=user;password=password"))
            {
                var input = "";
                Console.WriteLine("Enter a message. 'Quit' to quit.");
                while ((input = Console.ReadLine()) != "Quit")
                {
                    if (input == "loop")
                    {
                        for (int i = 0; i < 200000; i++)
                        {
                            bus.Publish(new TextMessage
                            {
                                Text = input + i
                            });
                        }
                    }

                    bus.Publish(new TextMessage
                    {
                        Text = input
                    });
                }
            }
        }
    }
}