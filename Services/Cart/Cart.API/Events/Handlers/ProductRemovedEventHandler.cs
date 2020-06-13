using Cart.API.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebShop.RabbitMQ;

namespace Cart.API.Events.Handlers
{
    public class ProductRemovedEventHandler : IMessageQueueEventHandler<ProductRemovedMessageEvent>
    {
        public CartContext CartContext { get; }

        public ProductRemovedEventHandler(CartContext cartContext)
        {
            CartContext = cartContext;
        }

        public Task HandleMessage(ProductRemovedMessageEvent productNameChanged)
        {
            return Task.Run(async () =>
            {
                var cartItemsAffected = CartContext.CartItems.Where(item => item.ProductId == productNameChanged.ProductId);
                CartContext.RemoveRange(cartItemsAffected);
                await CartContext.SaveChangesAsync();
            });

        }
    }
}
