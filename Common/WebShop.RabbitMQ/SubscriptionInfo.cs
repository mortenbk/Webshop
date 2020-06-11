﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WebShop.RabbitMQ
{
    public class SubscriptionInfo
    {
        public Type HandlerType { get; }

        private SubscriptionInfo(Type handlerType)
        {
            HandlerType = handlerType;
        }


        public static SubscriptionInfo Typed(Type handlerType)
        {
            return new SubscriptionInfo(handlerType);
        }
    }
}