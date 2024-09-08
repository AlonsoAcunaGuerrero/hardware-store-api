namespace hardware_store_api.Models.Responses
{
    public record UpdateProductResponse
    {
        public Product OldProduct { get; }
        public Product NewProduct { get; }

        public UpdateProductResponse(Product oldProduct, Product newProduct)
        {
            OldProduct = oldProduct;
            NewProduct = newProduct;
        }
    }
}
