namespace hardware_store_api.Models.Requests
{
    public record InsertProductRequest
    (string Name, string Description, decimal Price, int Stock, IFormFile Image, string Type);
}
