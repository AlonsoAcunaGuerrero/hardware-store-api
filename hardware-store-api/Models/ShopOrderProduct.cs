namespace hardware_store_api.Models
{
    public class ShopOrderProduct
    {
        public ShopOrder Order { get; }
        public Product Product { get; }
        public int Quantity { get; }

        public ShopOrderProduct(ShopOrder order, Product product, int quantity)
        {
            Order = order;
            Product = product;
            Quantity = quantity;
        }
    }
}
