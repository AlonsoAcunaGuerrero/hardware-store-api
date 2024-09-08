namespace hardware_store_api.Models
{
    public class ShopOrder
    {
        public string Id { get; }
        public User User { get; }
        public PaymentType PaymentType { get; }
        public Address Address { get; }
        public ShippingMethod ShippingMethod { get; }
        public decimal OrderTotal { get; }
        public OrderStatus OrderStatus {  get; }
        public DateTime CreationDate { get; }
        public DateTime UpdateDate { get; }

        public ShopOrder(string id, User user, PaymentType paymentType, Address address, 
            ShippingMethod shippingMethod, decimal orderTotal, OrderStatus orderStatus, 
            DateTime creationDate, DateTime updateDate)
        {
            Id = id;
            User = user;
            PaymentType = paymentType;
            Address = address;
            ShippingMethod = shippingMethod;
            OrderTotal = orderTotal;
            OrderStatus = orderStatus;
            CreationDate = creationDate;
            UpdateDate = updateDate;
        }
    }
}
