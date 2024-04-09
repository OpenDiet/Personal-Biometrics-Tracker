using PersonalBiometricsTracker.Dtos;
using PersonalBiometricsTracker.Entities;

namespace PersonalBiometricsTracker.Services
{
    public interface IBloodGlucoseService
    {
        Task<BloodGlucose> AddBloodGlucoseAsync(BloodGlucoseAddDto dto, int userId);
        Task<BloodGlucose> UpdateBloodGlucoseAsync(BloodGlucoseUpdateDto dto);
        Task<IEnumerable<BloodGlucose>> GetUserBloodGlucoseRecordsAsync(int userId);
    }
}