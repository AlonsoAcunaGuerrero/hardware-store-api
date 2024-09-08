namespace hardware_store_api.Models.Responses
{
    public record RegisterUserResponse
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string Username { get; }
        public string Email { get; }

        public RegisterUserResponse(string firstName, string lastName, string username, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Email = email;
        }
    }
}
