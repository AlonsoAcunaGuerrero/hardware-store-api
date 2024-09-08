namespace hardware_store_api.Models.Responses
{
    public record InsertProductResponse
    {
        public int Id { get; }
        public string Name { get; }
        public string Description { get; }
        public decimal Price { get; }
        public int Stock { get; }
        public string Image { get; }
        public ProductType Type { get; }

        public InsertProductResponse(int id, string name, string description, decimal price, int stock, string image, ProductType type)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
            Image = image;
            Type = type;
        }
    }
}
