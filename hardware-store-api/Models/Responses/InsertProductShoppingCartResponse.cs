namespace hardware_store_api.Models.Responses
{
    public record InsertProductShoppingCartResponse
    {
        public string Username { get; }
        public Product Product { get; }
        public int Quantity { get; }

        public InsertProductShoppingCartResponse(string username, Product product, int quantity)
        {
            Username = username;
            Product = product;
            Quantity = quantity;
        }

        public InsertProductShoppingCartResponse(User user, Product product, int quantity)
        {
            Username = user.Username;
            Product = product;
            Quantity = quantity;
        }
    }
}
