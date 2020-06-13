using User.API.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebShop.RabbitMQ;
using Microsoft.EntityFrameworkCore;

namespace User.API.Events.Handlers
{
    public class CartPriceChangedEventHandler : IMessageQueueEventHandler<CartPriceChangedMessageEvent>
    {
        public UserContext UserContext { get; }
        public IEventBus EventBus { get; }

        public CartPriceChangedEventHandler(UserContext cartContext, IEventBus eventBus)
        {
            UserContext = cartContext;
            EventBus = eventBus;
        }


        public Task HandleMessage(CartPriceChangedMessageEvent cartPriceChangeEvent)
        {
            return Task.Run(async () =>
            {
                var user = await UserContext.Users.FirstAsync(usr => usr.Id == cartPriceChangeEvent.UserId);
                if (String.IsNullOrEmpty(user?.Email))
                    return;
                EventBus.Publish(new SendEmailRequestMessageEvent()
                {
                    Email = user.Email,
                    Name = user.Name,
                    Subject = "Your cart has changes",
                    Body = 
$@"The product {cartPriceChangeEvent.CartItemName} has changes to its price.
The old price was {cartPriceChangeEvent.OldPrice}
The new price is {cartPriceChangeEvent.NewPrice}

------------
Best regards

Generic Webshop"
                });
            });

        }
    }
}
