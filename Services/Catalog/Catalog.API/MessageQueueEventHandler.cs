using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebShop.RabbitMQ;

namespace Catalog.API
{
    public class MessageQueueEventHandler : IMessageQueueEventHandler<MessageQueueEvent>
    {
        public Task HandleMessage(MessageQueueEvent message)
        {
            return Task.Run(() => Console.WriteLine($"Message Received: {message.Message}"));
        }
    }
}
