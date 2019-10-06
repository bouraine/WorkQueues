using System;
using System.Threading.Tasks;
using MassTransit;
using Messages;

namespace WebApiWorker.Controllers
{
    public class SendTextMessageConsumer : IConsumer<TextMessage>
    {
        public Task Consume(ConsumeContext<TextMessage> context)
        {
            Console.WriteLine(context.Message.Text);
            Console.ReadLine();
            return Task.CompletedTask;
        }
    }
}