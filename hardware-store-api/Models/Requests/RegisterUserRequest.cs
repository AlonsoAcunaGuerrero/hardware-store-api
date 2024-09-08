namespace hardware_store_api.Models.Requests
{
    public record RegisterUserRequest
    (string FirstName, string LastName, string Username, string Email, DateTime Birthdate, string Password, string Confirmpassword);
}
