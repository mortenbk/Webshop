using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.RabbitMQ
{
    public interface IMessageQueueEventHandler<T> where T : MessageQueueEvent
    {
        Task HandleMessage(T message);
    }
}
