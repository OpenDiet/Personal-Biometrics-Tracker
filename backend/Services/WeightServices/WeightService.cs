using Microsoft.EntityFrameworkCore;
using PersonalBiometricsTracker.Data;
using PersonalBiometricsTracker.Dtos;
using PersonalBiometricsTracker.Entities;

namespace PersonalBiometricsTracker.Services
{
    public class WeightService : IWeightService
    {
        private readonly PersonalBiometricsTrackerDbContext _context;

        public WeightService(PersonalBiometricsTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<Weight> AddWeightAsync(WeightAddDto weightDto, int userId)
        {
            var weight = new Weight
            {
                UserId = userId,
                Value = weightDto.Value,
                DateRecorded = weightDto.DateRecorded
            };

            _context.Weights.Add(weight);
            await _context.SaveChangesAsync();

            return weight;
        }

        public async Task<Weight> UpdateWeightAsync(WeightUpdateDto weightDto)
        {
            var weight = await _context.Weights.FindAsync(weightDto.Id);

            if (weight == null)
            {
                throw new Exception("Weight record not found.");
            }

            weight.Value = weightDto.Value;
            weight.DateRecorded = weightDto.DateRecorded;

            await _context.SaveChangesAsync();

            return weight;
        }

        public async Task<IEnumerable<Weight>> GetUserWeightsAsync(int userId)
        {
            return await _context.Weights.Where(w => w.UserId == userId).ToListAsync();
        }
    }
}