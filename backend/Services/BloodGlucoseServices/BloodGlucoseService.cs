using Microsoft.EntityFrameworkCore;
using PersonalBiometricsTracker.Data;
using PersonalBiometricsTracker.Dtos;
using PersonalBiometricsTracker.Entities;
using PersonalBiometricsTracker.Exceptions;

namespace PersonalBiometricsTracker.Services
{
    public class BloodGlucoseService : IBloodGlucoseService
    {
        private readonly PersonalBiometricsTrackerDbContext _context;

        public BloodGlucoseService(PersonalBiometricsTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<BloodGlucoseDto> AddBloodGlucoseAsync(int userId, BloodGlucoseAddDto dto)
        {

            if (dto.Value == null)
            {
                throw new ValidationException("Value was null. Value is required and cannot be null.");
            }

            if (dto.DateTimeRecorded == null)
            {
                throw new ValidationException("DateTimeRecorded was null. DateTimeRecorded is required and cannot be null.");
            }

            var record = new BloodGlucose
            {
                UserId = userId,
                Value = dto.Value.Value,
                DateTimeRecorded = dto.DateTimeRecorded.Value
            };

            _context.BloodGlucoses.Add(record);

            await _context.SaveChangesAsync();

            var responseDto = new BloodGlucoseDto
            {
                Id = record.Id,
                Value = record.Value,
                DateTimeRecorded = record.DateTimeRecorded,
                UserId = record.UserId
            };

            return responseDto;
        }

        public async Task<BloodGlucoseDto> UpdateBloodGlucoseAsync(int id, int userId, BloodGlucoseUpdateDto dto)
        {
            var record = await _context.BloodGlucoses.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (record == null || record.UserId != userId)
            {
                throw new NotFoundException("BloodGlucose entry not found or you do not have permission to update it.");
            }

            // If value is different, update it
            if (dto.Value != null && dto.Value != record.Value)
            {
                record.Value = dto.Value.Value;
            }

            // If DateTimeRecorded is different, update it
            if (dto.DateTimeRecorded != null && dto.DateTimeRecorded != record.DateTimeRecorded)
            {
                record.DateTimeRecorded = dto.DateTimeRecorded.Value;
            }

            await _context.SaveChangesAsync();

            var responseDto = new BloodGlucoseDto
            {
                Id = record.Id,
                Value = record.Value,
                DateTimeRecorded = record.DateTimeRecorded,
                UserId = record.UserId
            };

            return responseDto;
        }

        public async Task<IEnumerable<BloodGlucoseDto>> GetUserBloodGlucoseRecordsAsync(int userId)
        {
            var bloodGlucoses = await _context.BloodGlucoses
            .Where(b => b.UserId == userId)
            .Select(b => new BloodGlucoseDto
            {
                Id = b.Id,
                Value = b.Value,
                DateTimeRecorded = b.DateTimeRecorded,
                UserId = b.UserId
            })
            .ToListAsync();

            return bloodGlucoses;
        }
    }
}