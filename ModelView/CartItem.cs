using ShoeShop.Models;

namespace ShoeShop.ModelView
{
    public class CartItem
    {
        public Product product { get; set; }
        public int amount { get; set; }
        public double total => amount* product.Price;
    }
}
