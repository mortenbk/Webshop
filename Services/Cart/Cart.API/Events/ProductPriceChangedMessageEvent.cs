using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebShop.RabbitMQ;

namespace Cart.API.Events
{
    public class ProductPriceChangedMessageEvent : MessageQueueEvent
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
    }
}
