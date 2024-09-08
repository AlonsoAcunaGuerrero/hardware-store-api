namespace hardware_store_api.Models.Responses
{
    public record ProductQuantityResponse
    {
        public Product Product { get; }
        public int Quantity { get; }

        public ProductQuantityResponse(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }
}
