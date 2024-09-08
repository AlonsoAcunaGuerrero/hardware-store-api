namespace hardware_store_api.Models
{
    public class Address
    {
        public int Id { get; }
        public string UnitNumber { get; }
        public string StreetNumber { get; }
        public string AddressLine1 { get; }
        public string AddressLine2 { get; }
        public string PostalCode { get; }
        public City City { get; }

        public Address(int id, string unitNumber, string streetNumber, string addressLine1, string addressLine2, 
            string postalCode, City city)
        {
            Id = id;
            UnitNumber = unitNumber;
            StreetNumber = streetNumber;
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            PostalCode = postalCode;
            City = city;
        }
    }
}
