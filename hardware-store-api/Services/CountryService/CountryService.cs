using hardware_store_api.Connect;
using hardware_store_api.Models;
using hardware_store_api.Exceptions;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;

namespace hardware_store_api.Services.CountryService
{
    public class CountryService : ICountryService
    {
        public async Task<Country> GetByID(int id)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetCountryByID", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("CountryID", id);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int idCountry = reader.GetInt32("id_country");
                                string countryName = reader.GetString("country_name");

                                return new Country(idCountry, countryName);
                            }
                        }
                    }
                }

                throw new HttpStatusException(HttpStatusCode.NotFound, $"The country with ID '{id}' not exist.");
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, country couldn't be found.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting country by id.");
            }
        }

        public async Task<Country> GetByName(string name)
        {   
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetCountryByName", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("NameCountry", name);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int idCountry = reader.GetInt32("id_country");
                                string countryName = reader.GetString("country_name");

                                return new Country(idCountry, countryName);
                            }
                        }
                    }
                }

                throw new HttpStatusException(HttpStatusCode.NotFound, $"The country with name '{name}' not exist.");
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException ex)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, country couldn't be get.");
            }
            catch (Exception ex)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting country by name.");
            }
        }

        public async Task<List<Country>> GetList()
        {
            var countriesList = new List<Country>();

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetListCountries", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int idCountry = reader.GetInt32("id_country");
                                string countryName = reader.GetString("country_name");

                                var country = new Country(idCountry, countryName);

                                countriesList.Add(country);
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
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, list of countries couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting list of countries.");
            }

            return countriesList;
        }
    }
}
