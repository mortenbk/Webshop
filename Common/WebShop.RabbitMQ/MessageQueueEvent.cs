using System;
using System.Collections.Generic;
using System.Text;

namespace WebShop.RabbitMQ
{
    public partial class MessageQueueEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
