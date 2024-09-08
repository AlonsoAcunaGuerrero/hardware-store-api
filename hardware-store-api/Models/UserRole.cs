namespace hardware_store_api.Models
{
    public class UserRole
    {
        public int Id { get; }
        public string Name { get; }

        public UserRole(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
