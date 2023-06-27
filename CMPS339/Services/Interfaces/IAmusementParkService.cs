using CMPS339.Models;

namespace CMPS339.Services.Interfaces
{
    public interface IAmusementParkService
    {
        Task<List<Parks>> GetAllAsync();
        Task<Parks?> GetByIdAsync(int id);
        Task<ParksGetDto?> InsertAsync(ParksCreateDto dto);
    }
}
