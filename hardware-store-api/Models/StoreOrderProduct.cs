namespace hardware_store_api.Models
{
    public class StoreOrderProduct
    {
        public StoreOrder StoreOrder { get; }
        public Product Product { get; }
        public int Quantity { get; }

        public StoreOrderProduct(StoreOrder storeOrder, Product product, int quantity)
        {
            StoreOrder = storeOrder;
            Product = product;
            Quantity = quantity;
        }
    }
}
