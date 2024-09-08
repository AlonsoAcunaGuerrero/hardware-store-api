namespace hardware_store_api.Models
{
    public class ShoppingCart
    {
        public User User { get; }
        public Product Product { get; }
        public int Quantity { get; }

        public ShoppingCart(User user, Product product, int quantity)
        {
            User = user;
            Product = product;
            Quantity = quantity;
        }
    }
}
