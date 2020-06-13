
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebShop.RabbitMQ;

namespace Cart.API.Events
{
    public class CartPriceChangedMessageEvent : MessageQueueEvent
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public string CartItemName { get; set; }
        public decimal NewPrice { get; set; }
        public decimal OldPrice { get; set; }
    }
}
