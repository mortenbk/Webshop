using System;
using System.Collections;
using System.Collections.Generic;

namespace Cart.API.Models
{
    public partial class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public IList<CartItem> CartItems { get; set; }
    }
}
