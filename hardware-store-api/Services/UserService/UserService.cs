using hardware_store_api.Connect;
using hardware_store_api.Models;
using hardware_store_api.Exceptions;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;
using hardware_store_api.Services.UserRoleService;

namespace hardware_store_api.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRoleService _roleService;

        public UserService(IUserRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<List<User>> GetListUsers()
        {
            var usersList = new List<User>();

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetListUsers", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int idUser = reader.GetInt32("id_user");
                                string firstName = reader.GetString("first_name");
                                string lastName = reader.GetString("last_name");
                                string username = reader.GetString("username");
                                string password = reader.GetString("password");
                                string email = reader.GetString("email");
                                DateTime birthdate = reader.GetDateTime("birthdate");
                                int active = reader.GetInt32("is_active");

                                int idUserRole = reader.GetInt32("id_user_role");
                                var role = await _roleService.GetByID(idUserRole);

                                var user = new User(idUser, firstName, lastName, username, password, email, birthdate, active == 1, role);

                                usersList.Add(user);
                            }
                        }

                    }
                }
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, users couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting users.");
            }

            return usersList;
        }

        public async Task<User> GetByID(int id)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetUserByID", sql))
                    {
                        cmd.Parameters.AddWithValue("UserID", id);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int idUser = reader.GetInt32("id_user");
                                string firstName = reader.GetString("first_name");
                                string lastName = reader.GetString("last_name");
                                string username = reader.GetString("username");
                                string password = reader.GetString("password");
                                string email = reader.GetString("email");
                                DateTime birthdate = reader.GetDateTime("birthdate");
                                int active = reader.GetInt32("is_active");

                                int idUserRole = reader.GetInt32("id_user_role");
                                var role = await _roleService.GetByID(idUserRole);

                                return new User(idUser, firstName, lastName, username, password, email, birthdate, active == 1, role);
                            }
                        }
                    }
                }

                throw new HttpStatusException(HttpStatusCode.NotFound, $"The user with ID '{id}' not exist.");
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, user couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting user by id.");
            }
        }

        public async Task<User> GetByEmail(string email)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetUserByEmail", sql))
                    {
                        cmd.Parameters.AddWithValue("UserEmail", email);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int idUser = reader.GetInt32("id_user");
                                string firstName = reader.GetString("first_name");
                                string lastName = reader.GetString("last_name");
                                string username = reader.GetString("username");
                                string password = reader.GetString("password");
                                string userEmail = reader.GetString("email");
                                DateTime birthdate = reader.GetDateTime("birthdate");
                                int active = reader.GetInt32("is_active");

                                int idUserRole = reader.GetInt32("id_user_role");
                                var role = await _roleService.GetByID(idUserRole);

                                return new User(idUser, firstName, lastName, username, password, userEmail, birthdate, active == 1, role);
                            }
                        }

                    }
                }

                throw new HttpStatusException(HttpStatusCode.NotFound, String.Format("The user with that email not exist."));
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, user couldn't be get.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting user by email.");
            }
        }

        public async Task<User> GetUserLogin(string user_email, string user_password)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetUserLogin", sql))
                    {
                        cmd.Parameters.AddWithValue("LoginEmail", user_email);
                        cmd.Parameters.AddWithValue("LoginPassword", user_password);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int idUser = reader.GetInt32("id_user");
                                string firstName = reader.GetString("first_name");
                                string lastName = reader.GetString("last_name");
                                string username = reader.GetString("username");
                                string password = reader.GetString("password");
                                string email = reader.GetString("email");
                                DateTime birthdate = reader.GetDateTime("birthdate");
                                int active = reader.GetInt32("is_active");

                                int idUserRole = reader.GetInt32("id_user_role");
                                var role = await _roleService.GetByID(idUserRole);

                                return new User(idUser, firstName, lastName, username, password, email, birthdate, active == 1, role);
                            }
                        }

                    }
                }

                throw new HttpStatusException(HttpStatusCode.NotFound, String.Format("The user with that access data not exist."));
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, user couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting user to login.");
            }
        }

        public async Task<User> Insert(User user)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("InsertUser", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("NewUserFirstName", user.FirstName);
                        cmd.Parameters.AddWithValue("NewUserLastName", user.LastName);
                        cmd.Parameters.AddWithValue("NewUserName", user.Username);
                        cmd.Parameters.AddWithValue("NewUserPassword", user.Password);
                        cmd.Parameters.AddWithValue("NewUserEmail", user.Email);
                        cmd.Parameters.AddWithValue("NewUserBirthdate", user.Birthdate);
                        cmd.Parameters.AddWithValue("NewUserRole", user.Role.Id);

                        await cmd.ExecuteNonQueryAsync();
                    }

                    return await GetByEmail(user.Email);
                }
            }
            catch(MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, user couldn't be saved.");
            }
            catch(Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error saving user.");
            }
        }

        public async Task<User> Update(User user)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("UpdateUser", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("UserID", user.Id);
                        cmd.Parameters.AddWithValue("NewFirstName", user.FirstName);
                        cmd.Parameters.AddWithValue("NewLastName", user.LastName);
                        cmd.Parameters.AddWithValue("NewUsername", user.Username);
                        cmd.Parameters.AddWithValue("NewPassword", user.Password);
                        cmd.Parameters.AddWithValue("NewEmail", user.Email);
                        cmd.Parameters.AddWithValue("Activated", user.IsActive ? 1 : 0);
                        cmd.Parameters.AddWithValue("NewBirthdate", user.Birthdate);
                        cmd.Parameters.AddWithValue("NewRole", user.Role.Id);

                        await cmd.ExecuteNonQueryAsync();
                    }

                    return await GetByID(user.Id);

                }
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, user password couldn't be updated.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error updating user password.");
            }
        }

        public async Task<User> UpdatePassword(User user, string newPassword)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("UpdateUser", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("UserID", user.Id);
                        cmd.Parameters.AddWithValue("NewFirstName", user.FirstName);
                        cmd.Parameters.AddWithValue("NewLastName", user.LastName);
                        cmd.Parameters.AddWithValue("NewUsername", user.Username);
                        cmd.Parameters.AddWithValue("NewPassword", newPassword);
                        cmd.Parameters.AddWithValue("NewEmail", user.Email);
                        cmd.Parameters.AddWithValue("Activated", user.IsActive ? 1 : 0);
                        cmd.Parameters.AddWithValue("NewBirthdate", user.Birthdate);
                        cmd.Parameters.AddWithValue("NewRole", user.Role.Id);

                        await cmd.ExecuteNonQueryAsync();
                    }

                    return await GetByID(user.Id);
                    
                }
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, user's password couldn't be updated.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error updating user's password.");
            }
        }
        
        public async Task<User> UpdateActive(User user, bool active)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("UpdateUser", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("UserID", user.Id);
                        cmd.Parameters.AddWithValue("NewFirstName", user.FirstName);
                        cmd.Parameters.AddWithValue("NewLastName", user.LastName);
                        cmd.Parameters.AddWithValue("NewUsername", user.Username);
                        cmd.Parameters.AddWithValue("NewPassword", user.Password);
                        cmd.Parameters.AddWithValue("NewEmail", user.Email);
                        cmd.Parameters.AddWithValue("Activated", active ? 1 : 0);
                        cmd.Parameters.AddWithValue("NewBirthdate", user.Birthdate);
                        cmd.Parameters.AddWithValue("NewRole", user.Role.Id);
                        
                        await cmd.ExecuteNonQueryAsync();
                    }

                    return await GetByID(user.Id);
                }
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, user couldn't be enabling/disable.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error enabling/disabling user.");
            }
        }
    }
}
