namespace hardware_store_api.Models
{
    public class City
    {
        public int Id { get; }
        public string Name { get; }
        public Region Region { get; }

        public City(int id, string name, Region region)
        {
            Id = id;
            Name = name;
            Region = region;
        }
    }
}
