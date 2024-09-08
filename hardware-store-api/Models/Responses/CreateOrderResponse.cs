namespace hardware_store_api.Models.Responses
{
    public record CreateOrderResponse
    {
        public string Id { get; }

        public CreateOrderResponse(string id)
        {
            Id = id;
        }
    }
}
