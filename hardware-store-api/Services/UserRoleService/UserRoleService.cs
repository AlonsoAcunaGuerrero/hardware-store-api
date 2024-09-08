using hardware_store_api.Connect;
using hardware_store_api.Models;
using hardware_store_api.Exceptions;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;

namespace hardware_store_api.Services.UserRoleService
{
    public class UserRoleService : IUserRoleService
    {
        public async Task<List<UserRole>> GetList()
        {
            List<UserRole> rolesList = new List<UserRole>();

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetListUserRoles", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int idUserRole = reader.GetInt32("id_user_role");
                                string name = reader.GetString("name");

                                var role = new UserRole(idUserRole, name);

                                rolesList.Add(role);
                            }
                        }
                    }
                }
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, user roles couldn't be get.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting user roles.");
            }

            return rolesList;
        }

        public async Task<UserRole> GetByName(string name)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetUserRoleByName", sql))
                    {
                        cmd.Parameters.AddWithValue("UserRoleName", name);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int idUserRole = reader.GetInt32("id_user_role");
                                string roleName = reader.GetString("name");

                                return new UserRole(idUserRole, roleName);
                            }
                        }
                    }
                }
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, user role couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting user role by name.");
            }

            throw new HttpStatusException(HttpStatusCode.NotFound, $"The user role with name '{name}' not exist.");
        }

        public async Task<UserRole> GetByID(int id)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetUserRoleByID", sql))
                    {
                        cmd.Parameters.AddWithValue("UserRoleID", id);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int idUserRole = reader.GetInt32("id_user_role");
                                string name = reader.GetString("name");

                                return new UserRole(idUserRole, name);
                            }
                        }
                    }
                }
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, user role couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting user role by id.");
            }

            throw new HttpStatusException(HttpStatusCode.NotFound, $"The user role with ID '{id}' not exist.");
        }
    }
}
