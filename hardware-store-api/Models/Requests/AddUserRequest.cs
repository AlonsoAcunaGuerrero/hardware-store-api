namespace hardware_store_api.Models.Requests
{
    public record AddUserRequest
    (string FirstName, string LastName, string Username, string Email, DateTime Birthdate, string Password, string Role);
}