namespace hardware_store_api.Models
{
    public class User
    {
        public int Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Username { get; }
        public string Password { get; }
        public string Email { get; }
        public DateTime Birthdate { get; }
        public bool IsActive { get; }
        public UserRole Role { get; }

        public User(int id, string firstName, string lastName, string username, string password, string email, DateTime birthdate, bool isActive, UserRole role)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Password = password;
            Email = email;
            Birthdate = birthdate;
            IsActive = isActive;
            Role = role;
        }
    }
}
