using hardware_store_api.Connect;
using hardware_store_api.Exceptions;
using hardware_store_api.Models;
using hardware_store_api.Services.CityService;
using hardware_store_api.Services.CountryService;
using hardware_store_api.Services.RegionService;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;

namespace hardware_store_api.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly ICityService _cityService;

        public AddressService(ICityService cityService, ICountryService countryService, IRegionService regionService)
        {
            _cityService = cityService;
        }

        public async Task<Address> Insert(Address address)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("InsertAddress", sql))
                    {
                        
                        cmd.Parameters.AddWithValue("AddressUnitNumber", address.UnitNumber.Trim());
                        cmd.Parameters.AddWithValue("AddressStreetNumber", address.StreetNumber.Trim());
                        cmd.Parameters.AddWithValue("AddressAddressLine1", address.AddressLine1.Trim());
                        cmd.Parameters.AddWithValue("AddressAddressLine2", address.AddressLine2.Trim());
                        cmd.Parameters.AddWithValue("AddressPostalCode", address.PostalCode.Trim());
                        cmd.Parameters.AddWithValue("AddressIDCity", address.City.Id);
                        cmd.CommandType = CommandType.StoredProcedure;

                        var newIdAddress = await cmd.ExecuteScalarAsync() as int?;

                        if (newIdAddress != null)
                        {
                            return await GetByID(newIdAddress.Value);
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
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, address couldn't be saved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error saving address.");
            }

            return null;
        }

        public async Task LinkToUser(User user, Address address)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("InsertUserAddress", sql))
                    {
                        cmd.Parameters.AddWithValue("UserID", user.Id);
                        cmd.Parameters.AddWithValue("AddressID", address.Id);
                        
                        cmd.CommandType = CommandType.StoredProcedure;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, 
                    "Database Error, user couldn't be linked to that address.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError,
                    "Error linking user to address.");
            }
        }

        public async Task<Address> GetByID(int id)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetAddressByID", sql))
                    {
                        cmd.Parameters.AddWithValue("AddressID", id);
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                
                                int idAddress = reader.GetInt32("id_address");
                                string unitNumber = reader.GetString("unit_number");
                                string streetNumber = reader.GetString("street_number");
                                string addressLine1 = reader.GetString("address_line1");
                                string addressLine2 = reader.GetString("address_line2");
                                string postalCode = reader.GetString("postal_cod");

                                
                                int idCity = reader.GetInt32("id_city");
                                var city = await _cityService.GetByID(idCity);

                                var address = new Address(idAddress, unitNumber, streetNumber, addressLine1, addressLine2,
                                    postalCode, city);

                                return address;
                            }
                        }
                    }
                }

                throw new HttpStatusException(HttpStatusCode.NotFound, $"The address with ID '{id}' not exist.");
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, address couldn't be found.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting address.");
            }
        }

        public async Task<List<Address>> GetListByUser(User user)
        {
            var addressList = new List<Address>();

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetListAddressByUser", sql))
                    {
                        cmd.Parameters.AddWithValue("UserID", user.Id);
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int idAddress = reader.GetInt32("id_address");
                                var address = await GetByID(idAddress);

                                addressList.Add(address);
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
                throw new HttpStatusException(HttpStatusCode.InternalServerError,
                    "Database Error, list of user addresses couldn't be found.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError,
                    "Error getting list of user addresses.");
            }

            return addressList;
        }
        
        public async Task UnlinkToUser(User user, Address address)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("DeleteUserAddress", sql))
                    {
                        cmd.Parameters.AddWithValue("UserID", user.Id);
                        cmd.Parameters.AddWithValue("AddressID", address.Id);
                        cmd.CommandType = CommandType.StoredProcedure;

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, 
                    "Database Error, user couldn't be unlinked of that address.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError,
                    "Error unlinking address to user.");
            }
        }
    }
}
