namespace hardware_store_api.Models
{
    public class OrderStatus
    {
        public int Id { get; }
        public string Status { get; }

        public OrderStatus(int id, string status)
        {
            Id = id;
            Status = status;
        }
    }
}
