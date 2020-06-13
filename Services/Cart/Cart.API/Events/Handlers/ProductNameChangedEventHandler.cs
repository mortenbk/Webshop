using Cart.API.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebShop.RabbitMQ;

namespace Cart.API.Events.Handlers
{
    public class ProductNameChangedEventHandler : IMessageQueueEventHandler<ProductNameChangedMessageEvent>
    {
        public CartContext CartContext { get; }

        public ProductNameChangedEventHandler(CartContext cartContext)
        {
            CartContext = cartContext;
        }


        public Task HandleMessage(ProductNameChangedMessageEvent productNameChanged)
        {
            return Task.Run(async () =>
            {
                var cartItemsAffected = CartContext.CartItems.Where(item => item.ProductId == productNameChanged.ProductId);
                foreach (var cartItem in cartItemsAffected)
                {
                    cartItem.ProductName = productNameChanged.ProductName;
                }
                await CartContext.SaveChangesAsync();
            });

        }
    }
}
