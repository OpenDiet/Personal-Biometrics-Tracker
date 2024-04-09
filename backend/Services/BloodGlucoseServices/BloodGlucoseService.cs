using Microsoft.EntityFrameworkCore;
using PersonalBiometricsTracker.Data;
using PersonalBiometricsTracker.Dtos;
using PersonalBiometricsTracker.Entities;

namespace PersonalBiometricsTracker.Services
{
    public class BloodGlucoseService : IBloodGlucoseService
    {
        private readonly PersonalBiometricsTrackerDbContext _context;

        public BloodGlucoseService(PersonalBiometricsTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<BloodGlucose> AddBloodGlucoseAsync(BloodGlucoseAddDto dto, int userId)
        {
            var record = new BloodGlucose
            {
                UserId = userId,
                Value = dto.Value,
                DateTimeRecorded = dto.DateTimeRecorded
            };

            _context.BloodGlucoses.Add(record);

            await _context.SaveChangesAsync();

            return record;
        }

        public async Task<BloodGlucose> UpdateBloodGlucoseAsync(BloodGlucoseUpdateDto dto)
        {
            var record = await _context.BloodGlucoses.FindAsync(dto.Id);
            if (record == null || record.UserId != dto.UserId)
            {
                throw new KeyNotFoundException("Record not found or user mismatch.");
            }

            record.Value = dto.Value;
            record.DateTimeRecorded = dto.DateTimeRecorded;
            await _context.SaveChangesAsync();

            return record;
        }

        public async Task<IEnumerable<BloodGlucose>> GetUserBloodGlucoseRecordsAsync(int userId)
        {
            return await _context.BloodGlucoses.Where(b => b.UserId == userId).ToListAsync();
        }
    }
}