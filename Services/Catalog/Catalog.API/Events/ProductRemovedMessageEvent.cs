using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebShop.RabbitMQ;

namespace Catalog.API.Events
{
    public class ProductRemovedMessageEvent : MessageQueueEvent
    {
        public int ProductId { get; set; }
    }
}
