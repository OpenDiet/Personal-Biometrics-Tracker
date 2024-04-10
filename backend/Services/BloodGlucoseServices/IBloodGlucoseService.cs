using PersonalBiometricsTracker.Dtos;

namespace PersonalBiometricsTracker.Services
{
    public interface IBloodGlucoseService
    {
        Task<BloodGlucoseDto> AddBloodGlucoseAsync(int userId, BloodGlucoseAddDto dto);
        Task<BloodGlucoseDto> UpdateBloodGlucoseAsync(int id, int userId, BloodGlucoseUpdateDto dto);
        Task<IEnumerable<BloodGlucoseDto>> GetUserBloodGlucoseRecordsAsync(int userId);
    }
}