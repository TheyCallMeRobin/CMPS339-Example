using CMPS339.Models;
using CMPS339.Services.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace CMPS339.Services.Implementations
{
    public class AmusementParkService : IAmusementParkService
    {
        private readonly ILogger<AmusementParkService> _logger;
        private readonly IMemoryCache _cache;
        public AmusementParkService(ILogger<AmusementParkService> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public async Task<List<Parks>> GetAllAsync()
        {
            List<Parks> parks = new();
            const string key = "parks-list";

            if (_cache.TryGetValue(key, out List<Parks> parksList))
            {
                return parksList;
            }

            using (IDbConnection connection = new SqlConnection(ConnectionService.ConnectionString))
            {
               connection.Open();

               var parkData =  await connection.QueryAsync<Parks>("SELECT * FROM Parks WHERE Id = @Id", new { Id = 1 });

               parks = parkData.ToList();
            }

            _cache.Set(key, parks, TimeSpan.FromSeconds(10));
            return parks;
        }

        public async Task<Parks?> GetByIdAsync(int id)
        {
            List<Parks> parks = new();

            using (IDbConnection connection = new SqlConnection(ConnectionService.ConnectionString))
            {
                connection.Open();
                var parkData = await connection.QueryAsync<Parks>("SELECT * FROM Parks WHERE Id = @Id", new { Id = id });

                parks = parkData.ToList();
            }

            return parks.FirstOrDefault();
        }

        public async Task<ParksGetDto?> InsertAsync(ParksCreateDto dto)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(ConnectionService.ConnectionString);
                connection.Open();

                IEnumerable<Parks> newPark = await connection
                    .QueryAsync<Parks>("INSERT INTO PARKS OUTPUT INSERTED.* VALUES (@Name) ", new { Name = dto.Name });

                return newPark
                    .Select(x => new ParksGetDto 
                    { 
                        Id = x.Id,
                        Name = x.Name 
                    })
                    .FirstOrDefault();
            } catch (Exception e)
            {
                _logger.LogError(e, "An error has ocurred. DTO Value Name: {NAME} At: {TIME}", dto.Name, DateTime.Now.ToString());
                return null;
            }
        }
    }
}
