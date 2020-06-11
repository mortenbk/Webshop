using System;
using System.Collections.Generic;
using System.Text;

namespace WebShop.RabbitMQ
{
    public interface IEventBus
    {
        void Publish(MessageQueueEvent @event);

        void Subscribe<T, TH>()
            where T : MessageQueueEvent
            where TH : IMessageQueueEventHandler<T>;

        void Unsubscribe<T, TH>()
            where TH : IMessageQueueEventHandler<T>
            where T : MessageQueueEvent;
    }
}
