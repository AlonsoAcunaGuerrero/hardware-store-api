namespace hardware_store_api.Models.Requests
{
    public record LoginUserRequest
    (
        string Email,
        string Password
     );
}
