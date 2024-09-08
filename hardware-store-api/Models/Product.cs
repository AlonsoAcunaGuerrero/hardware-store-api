namespace hardware_store_api.Models
{
    public class Product
    {
        public int Id { get; }
        public string Name { get; }
        public string Description { get; }
        public decimal Price { get; }
        public int Stock { get; }
        public byte[] Image { get; }
        public ProductType Type { get; }

        public Product(int id, string name, string description, decimal price, int stock, byte[] image, ProductType type)
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
