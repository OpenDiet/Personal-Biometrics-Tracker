using PersonalBiometricsTracker.Dtos;
using PersonalBiometricsTracker.Entities;

namespace PersonalBiometricsTracker.Services
{
    public interface IWeightService
    {
        Task<Weight> AddWeightAsync(WeightAddDto weightDto, int userId);
        Task<Weight> UpdateWeightAsync(WeightUpdateDto weightDto);
        Task<IEnumerable<Weight>> GetUserWeightsAsync(int userId);
    }
}