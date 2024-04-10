using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using PersonalBiometricsTracker.Data;
using PersonalBiometricsTracker.Dtos;
using PersonalBiometricsTracker.Entities;
using PersonalBiometricsTracker.Exceptions;

namespace PersonalBiometricsTracker.Services
{
    public class WeightService : IWeightService
    {
        private readonly PersonalBiometricsTrackerDbContext _context;

        public WeightService(PersonalBiometricsTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<WeightDto> AddWeightAsync(WeightAddDto weightDto, int userId)
        {

            if (weightDto.Value == null)
            {
                throw new Exceptions.ValidationException("Weight Value was null. Weight Value is required and cannot be null.");
            }

            if (weightDto.DateRecorded == null)
            {
                throw new Exceptions.ValidationException("Weight DateRecorded was null. Weight DateRecorded is required and cannot be null.");
            }

            var weight = new Weight
            {
                UserId = userId,
                Value = weightDto.Value.Value,
                DateRecorded = weightDto.DateRecorded.Value
            };
            _context.Weights.Add(weight);
            await _context.SaveChangesAsync();

            var responseDto = new WeightDto
            {
                Id = weight.Id,
                Value = weight.Value,
                DateRecorded = weight.DateRecorded,
                UserId = weight.UserId
            };

            return responseDto;
        }

        public async Task<WeightDto> UpdateWeightAsync(int id, int userId, WeightUpdateDto weightDto)
        {
            // Attempt to find the weight record by ID and ensure it belongs to the specified userId
            var weight = await _context.Weights.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (weight == null)
            {
                throw new NotFoundException("Weight record not found or you do not have permission to update it.");
            }

            // If the weight value was changed, update it: 
            if (weightDto.Value != null && weightDto.Value != weight.Value)
            {
                weight.Value = weightDto.Value.Value;
            }

            // If the date recorded was changed, update it: 
            if (weightDto.DateRecorded != null && weightDto.DateRecorded != weight.DateRecorded)
            {
                weight.DateRecorded = weightDto.DateRecorded.Value;
            }

            await _context.SaveChangesAsync();

            var responseDto = new WeightDto
            {
                Id = weight.Id,
                Value = weight.Value,
                DateRecorded = weight.DateRecorded,
                UserId = weight.UserId
            };

            return responseDto;
        }

        public async Task<IEnumerable<WeightDto>> GetUserWeightsAsync(int userId)
        {
            var weights = await _context.Weights
                .Where(w => w.UserId == userId)
                .Select(w => new WeightDto
                {
                    Id = w.Id,
                    Value = w.Value,
                    DateRecorded = w.DateRecorded,
                    UserId = w.UserId
                })
                .ToListAsync();

            return weights;
        }
    }
}