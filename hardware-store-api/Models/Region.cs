namespace hardware_store_api.Models
{
    public class Region
    {
        public int Id { get; }
        public string Name { get; }
        public Country Country { get; }

        public Region(int id, string name, Country country)
        {
            Id = id;
            Name = name;
            Country = country;
        }
    }
}
