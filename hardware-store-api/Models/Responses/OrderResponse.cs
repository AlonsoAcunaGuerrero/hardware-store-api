namespace hardware_store_api.Models.Responses
{
    public record OrderResponse
    {
        public string Id { get; }
        public string ClientFirstName { get; }
        public string ClientLastName { get; }
        public string ClientEmail { get; }
        public PaymentType PaymentType { get; }
        public string Address { get; }
        public City City { get; }
        public ShippingMethod ShippingMethod { get; }
        public List<ProductQuantityResponse> ProductsList { get; }
        public decimal OrderTotal { get; }
        public DateTime CreationDate { get; }
        public DateTime UpdateDate {  get; }
        public OrderStatus OrderStatus { get; }

        public OrderResponse(string id, string clientFirstName, string clientLastName, string clientEmail, PaymentType paymentType, 
            string address, City city, ShippingMethod shippingMethod, 
            List<ProductQuantityResponse> productsList, decimal orderTotal, 
            DateTime creationDate, DateTime updateDate, OrderStatus orderStatus)
        {
            Id = id;
            ClientFirstName = clientFirstName;
            ClientLastName = clientLastName;
            ClientEmail = clientEmail;
            PaymentType = paymentType;
            Address = address;
            City = city;
            ShippingMethod = shippingMethod;
            ProductsList = productsList;
            OrderTotal = orderTotal;
            CreationDate = creationDate;
            UpdateDate = updateDate;
            OrderStatus = orderStatus;
        }

        public OrderResponse(string id, User user, PaymentType paymentType,
            Address address, ShippingMethod shippingMethod, List<ProductQuantityResponse> productsList, 
            decimal orderTotal, DateTime creationDate, DateTime updateDate, OrderStatus orderStatus)
        {
            Id = id;
            ClientFirstName = user.FirstName;
            ClientLastName = user.LastName;
            ClientEmail = user.Email;
            PaymentType = paymentType;
            Address = "[" + address.UnitNumber + "] " + address.AddressLine1 + ", " + address.AddressLine2 + " - " + 
                address.StreetNumber + ", Postal Code: " + address.PostalCode;
            City = address.City;
            ShippingMethod = shippingMethod;
            ProductsList = productsList;
            OrderTotal = orderTotal;
            CreationDate = creationDate;
            UpdateDate = updateDate;
            OrderStatus = orderStatus;
        }
    }
}
