using hardware_store_api.Models;

namespace hardware_store_api.Services.UserService
{
    public interface IUserService
    {
        Task<List<User>> GetListUsers();
        Task<User> GetByID(int id);
        Task<User> GetByEmail(string email);
        Task<User> GetUserLogin(string email, string password);
        Task<User> Insert(User user);
        Task<User> UpdatePassword(User user, string newPassword);
        Task<User> UpdateActive(User user, bool active);


        static public string HideEmailCharacters(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return email;
            }

            int arrobaIndex = email.IndexOf('@');
            if (arrobaIndex >= 0)
            {
                string parteOculta = new string('*', arrobaIndex);
                string parteVisible = email.Substring(arrobaIndex);
                return parteOculta + parteVisible;
            }

            return email;
        }
    }
}
