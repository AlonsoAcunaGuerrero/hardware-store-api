namespace hardware_store_api.Models.Requests
{
    public record CreateStoreOrderRequest
    (string ClientFirstName, string ClientLastName, string ClientEmail, string Address, string City,
    string ShippingMethod, List<ProductQuantityRequest> ProductsList);

    public record ProductQuantityRequest
    (string Name, int Quantity);
}
