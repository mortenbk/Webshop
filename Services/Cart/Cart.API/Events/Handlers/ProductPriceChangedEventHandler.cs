using Cart.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebShop.RabbitMQ;

namespace Cart.API.Events.Handlers
{
    public class ProductPriceChangedEventHandler : IMessageQueueEventHandler<ProductPriceChangedMessageEvent>
    {
        public CartContext CartContext { get; }
        public IEventBus EventBus { get; }

        public ProductPriceChangedEventHandler(CartContext cartContext, IEventBus eventBus)
        {
            CartContext = cartContext;
            EventBus = eventBus;
        }


        public Task HandleMessage(ProductPriceChangedMessageEvent productPriceChanged)
        {
            return Task.Run(async () =>
            {
                var cartItemsAffected = CartContext.CartItems.Where(item => item.ProductId == productPriceChanged.ProductId);
                foreach (var cartItem in cartItemsAffected)
                {
                    var oldPrice = cartItem.Price;
                    if (productPriceChanged.Price != oldPrice)
                    {
                        cartItem.Price = productPriceChanged.Price;
                        var cartParent = await CartContext.Carts.FirstOrDefaultAsync(cart => cart.Id == cartItem.CartId);
                        // No user information, don't send email
                        if (cartParent == null)
                            return;
                        //Send email about price change
                        EventBus.Publish(new CartPriceChangedMessageEvent() { CartItemName = cartItem.ProductName, NewPrice = cartItem.Price, OldPrice = oldPrice, UserId = cartParent.UserId, CartId = cartParent.Id });
                    }
                }
                await CartContext.SaveChangesAsync();
            });

        }
    }
}
