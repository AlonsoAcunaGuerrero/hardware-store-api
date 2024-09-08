namespace hardware_store_api.Models.Responses
{
    public record UserResponse
    {
        public int IdUser { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Username { get; }
        public string Email { get; }
        public bool IsActive { get; }
        public UserRole Role { get; }

        public UserResponse(int idUser, string firstName, string lastName, string username, string email, 
            bool isActive, UserRole role)
        {
            IdUser = idUser;
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Email = email;
            IsActive = isActive;
            Role = role;
        }

        public UserResponse(User user)
        {
            IdUser = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            Email = user.Email;
            IsActive = user.IsActive;
            Role = user.Role;
        }
    }
}
