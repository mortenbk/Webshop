using System;
using System.Collections.Generic;
using System.Text;

namespace WebShop.RabbitMQ
{
    public interface ISubscriptionManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;
        void AddSubscription<T, TH>()
           where T : MessageQueueEvent
           where TH : IMessageQueueEventHandler<T>;

        void RemoveSubscription<T, TH>()
             where TH : IMessageQueueEventHandler<T>
             where T : MessageQueueEvent;

        bool HasSubscriptionsForEvent<T>() where T : MessageQueueEvent;
        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : MessageQueueEvent;
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        string GetEventKey<T>();
    }
}
