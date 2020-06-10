using System;
using System.Collections.Generic;
using System.Text;

namespace Cart.Models
{
    public partial class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
