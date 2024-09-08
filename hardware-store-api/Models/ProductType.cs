namespace hardware_store_api.Models
{
    public class ProductType
    {
        public int Id { get; }
        public string Name { get; }
        public ProductType Parent { get; }

        public ProductType(int id, string name)
        {
            Id = id;
            Name = name;
            Parent = null;
        }

        public ProductType(int id, string name, ProductType parent)
        {
            Id = id;
            Name = name;
            Parent = parent;
        }

    }
}