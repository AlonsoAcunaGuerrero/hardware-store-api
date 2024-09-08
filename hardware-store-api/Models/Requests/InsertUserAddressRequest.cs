namespace hardware_store_api.Models.Requests
{
    public record InsertUserAddressRequest
    (string UnitNumber, string StreetNumber, string AddressLine1, string AddressLine2, string PostalCode, string CityName);
}
