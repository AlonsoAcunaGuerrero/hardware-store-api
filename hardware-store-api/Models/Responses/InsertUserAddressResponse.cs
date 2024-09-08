namespace hardware_store_api.Models.Responses
{
    public record InsertUserAddressResponse
    {
        public UserResponse User { get; }
        public Address Address { get; }

        public InsertUserAddressResponse(UserResponse user, Address address)
        {
            User = user;
            Address = address;
        }
    }
}
