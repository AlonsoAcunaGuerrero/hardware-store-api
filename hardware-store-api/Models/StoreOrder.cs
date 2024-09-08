namespace hardware_store_api.Models
{
    public class StoreOrder
    {
        public string Id { get; }
        public string ClientFirstName { get; }
        public string ClientLastName { get; }
        public string ClientEmail { get; }
        public PaymentType PaymentType { get; }
        public string Address { get; }
        public City City { get; }
        public ShippingMethod ShippingMethod { get; }
        public decimal OrderTotal { get; }
        public OrderStatus OrderStatus { get; }
        public DateTime CreationDate { get; }
        public DateTime UpdateDate { get; }

        public StoreOrder(string id, string clientFirstName, string clientLastName, string clientEmail, PaymentType paymentType, 
            string address, City city, ShippingMethod shippingMethod, decimal orderTotal, 
            OrderStatus orderStatus, DateTime creationDate, DateTime updateDate)
        {
            Id = id;
            ClientFirstName = clientFirstName;
            ClientLastName = clientLastName;
            ClientEmail = clientEmail;
            PaymentType = paymentType;
            Address = address;
            City = city;
            ShippingMethod = shippingMethod;
            OrderTotal = orderTotal;
            OrderStatus = orderStatus;
            CreationDate = creationDate;
            UpdateDate = updateDate;
        }
    }
}
