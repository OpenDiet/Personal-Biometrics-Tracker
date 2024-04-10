using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PersonalBiometricsTracker.Data;
using PersonalBiometricsTracker.Dtos;
using PersonalBiometricsTracker.Entities;
using PersonalBiometricsTracker.Exceptions;
using PersonalBiometricsTracker.Services;

namespace PersonalBiometricsTracker.Tests
{

    public class BloodGlucoseServiceTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<PersonalBiometricsTrackerDbContext> _options;
        private readonly PersonalBiometricsTrackerDbContext _context;

        public BloodGlucoseServiceTests()
        {
            // Initialize the in-memory SQLite database and open connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            _options = new DbContextOptionsBuilder<PersonalBiometricsTrackerDbContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new PersonalBiometricsTrackerDbContext(_options);

            // Ensure the database is created
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            // Dispose resources when tests are finished
            _context.Dispose();
            _connection.Close();
        }

        [Fact]
        public async Task AddBloodGlucoseAsync_ValidData_AddsRecord()
        {
            // Use _context directly in your test without repeating the setup code
            var service = new BloodGlucoseService(_context);
            var dto = new BloodGlucoseAddDto { Value = 5.6m, DateTimeRecorded = DateTime.Now };

            // Act
            await service.AddBloodGlucoseAsync(1, dto);

            // Assert
            var addedEntry = await _context.BloodGlucoses.FirstOrDefaultAsync(b => b.UserId == 1);
            Assert.NotNull(addedEntry);
            Assert.Equal(dto.Value, addedEntry.Value);
            Assert.Equal(dto.DateTimeRecorded.Value.Date, addedEntry.DateTimeRecorded.Date);
        }

        [Fact]
        public async Task UpdateBloodGlucoseAsync_ValidData_UpdatesRecord()
        {
            var userId = 1;
            var updatedValue = 7.8m;
            var updatedDateTime = DateTime.UtcNow;

            var existingRecord = new BloodGlucose { UserId = userId, Value = 5.6m, DateTimeRecorded = DateTime.Now.AddDays(-1) };
            _context.BloodGlucoses.Add(existingRecord);
            _context.SaveChanges();

            var service = new BloodGlucoseService(_context);
            var dto = new BloodGlucoseUpdateDto { Value = updatedValue, DateTimeRecorded = updatedDateTime };

            // Act
            await service.UpdateBloodGlucoseAsync(existingRecord.Id, userId, dto);

            // Assert
            var updatedEntry = await _context.BloodGlucoses.FirstOrDefaultAsync(b => b.Id == existingRecord.Id);
            Assert.NotNull(updatedEntry);
            Assert.Equal(updatedValue, updatedEntry.Value);
            Assert.Equal(updatedDateTime, updatedEntry.DateTimeRecorded);
        }

        [Fact]
        public async Task UpdateBloodGlucoseAsync_RecordNotFound_ThrowsNotFoundException()
        {
            var service = new BloodGlucoseService(_context);
            var dto = new BloodGlucoseUpdateDto { Value = 6.9m, DateTimeRecorded = DateTime.UtcNow };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateBloodGlucoseAsync(99, 1, dto));
        }

        [Fact]
        public async Task AddBloodGlucoseAsync_InvalidData_Null_ThrowsValidationException()
        {
            var service = new BloodGlucoseService(_context);
            var dto = new BloodGlucoseAddDto { };

            await Assert.ThrowsAsync<ValidationException>(() => service.AddBloodGlucoseAsync(1, dto));
        }

        [Fact]
        public async Task AddBloodGlucoseAsync_UserDoesNotExist_ThrowsDbUpdateException()
        {
            var service = new BloodGlucoseService(_context);
            var dto = new BloodGlucoseAddDto { Value = 100, DateTimeRecorded = DateTime.UtcNow };

            await Assert.ThrowsAsync<DbUpdateException>(() => service.AddBloodGlucoseAsync(9999, dto));
        }

    }

}