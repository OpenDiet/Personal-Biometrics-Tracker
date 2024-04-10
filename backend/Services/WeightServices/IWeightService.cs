using PersonalBiometricsTracker.Dtos;
using PersonalBiometricsTracker.Entities;

namespace PersonalBiometricsTracker.Services
{
    public interface IWeightService
    {
        Task<WeightDto> AddWeightAsync(WeightAddDto weightDto, int userId);
        Task<WeightDto> UpdateWeightAsync(int id, int userId, WeightUpdateDto weightDto);
        Task<IEnumerable<WeightDto>> GetUserWeightsAsync(int userId);
    }
}