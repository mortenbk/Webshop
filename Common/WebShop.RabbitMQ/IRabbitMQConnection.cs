using RabbitMQ.Client;
using System;

namespace WebShop.RabbitMQ
{
    public interface IRabbitMQConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
