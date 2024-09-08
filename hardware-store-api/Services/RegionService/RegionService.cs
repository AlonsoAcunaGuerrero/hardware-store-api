using hardware_store_api.Connect;
using hardware_store_api.Exceptions;
using hardware_store_api.Models;
using hardware_store_api.Services.CountryService;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;

namespace hardware_store_api.Services.RegionService
{
    public class RegionService : IRegionService
    {
        private readonly ICountryService _countryService;

        public RegionService(ICountryService countryService)
        {
            _countryService = countryService;
        }

        public async Task<Region> GetByID(int id)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetRegionByID", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("RegionID", id);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int idRegion = reader.GetInt32("id_region");
                                string regionName = reader.GetString("region_name");

                                int idCountry = reader.GetInt32("id_country");
                                var country = await _countryService.GetByID(idCountry);

                                return new Region(idRegion, regionName, country);
                            }
                        }

                    }
                }

                throw new HttpStatusException(HttpStatusCode.NotFound, $"The region with ID '{id}' not exist.");
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, region couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting region by id.");
            }
        }

        public async Task<Region> GetByName(string name)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetRegionByName", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("NameRegion", name);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int idRegion = reader.GetInt32("id_region");
                                string regionName = reader.GetString("region_name");

                                int idCountry = reader.GetInt32("id_country");
                                var country = await _countryService.GetByID(idCountry);

                                return new Region(idRegion, regionName, country);
                            }
                        }

                    }
                }

                throw new HttpStatusException(HttpStatusCode.NotFound, $"The region with name '{name}' not exist.");
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, region couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting region by name.");
            }
        }

        public async Task<List<Region>> GetListByCountry(Country country)
        {
            var regionsList = new List<Region>();

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    using (var cmd = new MySqlCommand("GetListRegionsByCountry", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("CountryID", country.Id);

                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int idRegion = reader.GetInt32("id_region");
                                string regionName = reader.GetString("region_name");

                                var region = new Region(idRegion, regionName, country);

                                regionsList.Add(region);
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
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, list of regions couldn't be get.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting list of regions.");
            }

            return regionsList;
        }
    }
}
