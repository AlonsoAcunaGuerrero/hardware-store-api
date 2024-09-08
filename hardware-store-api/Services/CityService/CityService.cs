using hardware_store_api.Connect;
using hardware_store_api.Models;
using hardware_store_api.Exceptions;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics.Metrics;
using System.Net;
using hardware_store_api.Services.RegionService;

namespace hardware_store_api.Services.CityService
{
    public class CityService : ICityService
    {
        private readonly IRegionService _regionService;

        public CityService(IRegionService regionService)
        {
            _regionService = regionService;
        }

        public async Task<List<City>> GetListByRegion(Region region)
        {
            var citiesList = new List<City>();

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetListCitiesByRegion", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("RegionID", region.Id);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int idCity = reader.GetInt32("id_city");
                                string cityName = reader.GetString("city_name");

                                var city = new City(idCity, cityName, region);

                                citiesList.Add(city);
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
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, cities couldn't be found.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting cities.");
            }

            return citiesList;
        }

        public async Task<City> GetByID(int id)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {

                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetCityByID", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("CityID", id);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int idCity = reader.GetInt32("id_city");
                                string cityName = reader.GetString("city_name");

                                int idRegion = reader.GetInt32("id_region");
                                var region = await _regionService.GetByID(idRegion);

                                return new City(idCity, cityName, region);
                            }
                        }

                    }
                }

                throw new HttpStatusException(HttpStatusCode.NotFound, $"The city with ID '{id}' not exist.");
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, city couldn't be found.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting city by id.");
            }
        }
        
        public async Task<City> GetByName(string name)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetCityByName", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("NameCity", name);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int idCity = reader.GetInt32("id_city");
                                string cityName = reader.GetString("city_name");

                                int idRegion = reader.GetInt32("id_region");
                                var region = await _regionService.GetByID(idRegion);

                                return new City(idCity, cityName, region);
                            }
                        }
                    }
                }

                throw new HttpStatusException(HttpStatusCode.NotFound, $"The city with name '{name}' not exist.");
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, city couldn't be found.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting city by name.");
            }
        }
    }
}
