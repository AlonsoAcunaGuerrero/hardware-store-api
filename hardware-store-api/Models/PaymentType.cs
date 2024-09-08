namespace hardware_store_api.Models
{
    public class PaymentType
    {
        public int Id { get; }
        public string Name { get; }

        public PaymentType(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
