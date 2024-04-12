using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PersonalBiometricsTracker.Data;
using PersonalBiometricsTracker.Dtos;
using PersonalBiometricsTracker.Entities;
using PersonalBiometricsTracker.Exceptions;
using PersonalBiometricsTracker.Services;
using SQLitePCL;

namespace PersonalBiometricsTracker.Tests
{
    public class WeightServiceTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<PersonalBiometricsTrackerDbContext> _options;
        private readonly PersonalBiometricsTrackerDbContext _context;

        public WeightServiceTests()
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
        public async Task AddWeightAsync_ValidData_AddsRecord()
        {
            // Arrange
            var user = new User { Username = "TestUser", Email = "test@test.com", Password = "TestPassword" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var service = new WeightService(_context);
            var dto = new WeightAddDto { Value = 100, DateRecorded = DateTime.UtcNow };

            // Act
            await service.AddWeightAsync(dto, user.Id);

            // Assert
            var addedEntry = await _context.Weights.FirstOrDefaultAsync(w => w.UserId == user.Id);
            Assert.NotNull(addedEntry);
            Assert.Equal(dto.Value, addedEntry.Value);
            Assert.Equal(dto.DateRecorded, addedEntry.DateRecorded);

        }

        [Fact]
        public async Task AddWeightAsync_InvalidData_Null_ThrowsValidationException()
        {
            var service = new WeightService(_context);
            var dto = new WeightAddDto { };

            await Assert.ThrowsAsync<ValidationException>(() => service.AddWeightAsync(dto, 1));
        }

        [Fact]
        public async Task AddWeightAsync_UserDoesNotExist_ThrowsDbUpdateException()
        {
            var service = new WeightService(_context);
            var dto = new WeightAddDto { Value = 100, DateRecorded = DateTime.UtcNow };

            await Assert.ThrowsAsync<DbUpdateException>(() => service.AddWeightAsync(dto, 999));
        }

        [Fact]
        public async Task UpdateWeightAsync_ValidData_UpdatesRecord()
        {
            var user = new User { Username = "TestUser", Email = "test@test.com", Password = "TestPassword" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var updatedValue = 200;
            var updatedDateTime = DateTime.UtcNow;

            var existingRecord = new Weight { UserId = user.Id, Value = 100, DateRecorded = DateTime.UtcNow };
            _context.Weights.Add(existingRecord);
            await _context.SaveChangesAsync();

            var service = new WeightService(_context);
            var dto = new WeightUpdateDto { Value = updatedValue, DateRecorded = updatedDateTime };

            await service.UpdateWeightAsync(existingRecord.Id, user.Id, dto);

            var updatedEntry = await _context.Weights.FirstOrDefaultAsync(w => w.UserId == user.Id);
            Assert.NotNull(updatedEntry);
            Assert.Equal(updatedValue, updatedEntry.Value);
            Assert.Equal(updatedDateTime, updatedEntry.DateRecorded);

        }

        [Fact]
        public async Task UpdateWeightAsync_RecordNotFound_ThrowsNotFoundException()
        {
            var service = new WeightService(_context);
            var dto = new WeightUpdateDto { Value = 100, DateRecorded = DateTime.UtcNow };

            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateWeightAsync(100, 1, dto));
        }

        [Fact]
        public async Task UpdateWeightAsync_UpdateAnotherUsersRecords_ThrowsNotFoundException()
        {
            var authedUser = new User { Username = "AuthedUser", Email = "auth@test.com", Password = "TestPassword:" };
            var notAuthedUser = new User { Username = "NotAuthedUser", Email = "notauth@test.com", Password = "TestPassword" };
            _context.Users.Add(authedUser);
            _context.Users.Add(notAuthedUser);
            await _context.SaveChangesAsync();

            var service = new WeightService(_context);
            var addDto = new WeightAddDto { Value = 100, DateRecorded = DateTime.UtcNow };
            var updateDto = new WeightUpdateDto { Value = 200, DateRecorded = DateTime.UtcNow };

            var notAuthedUserWEntry = await service.AddWeightAsync(addDto, notAuthedUser.Id);

            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateWeightAsync(notAuthedUserWEntry.Id, authedUser.Id, updateDto));
        }

        [Fact]
        public async Task GetUserWeightRecordsAsync_ValidUser_ReturnsCorrectRecords()
        {
            var user = new User { Username = "User1", Email = "user1@test.com", Password = "TestPassword" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var service = new WeightService(_context);

            var weightRecords = new List<Weight> {
                new Weight { UserId = user.Id, Value = 100, DateRecorded = DateTime.UtcNow },
                new Weight { UserId = user.Id, Value = 150, DateRecorded = DateTime.UtcNow }

            };

            await _context.Weights.AddRangeAsync(weightRecords);
            await _context.SaveChangesAsync();

            var results = await service.GetUserWeightsAsync(user.Id);

            Assert.Equal(2, results.Count());
            foreach (var record in results)
            {
                Assert.Contains(weightRecords, w => w.Value == record.Value && w.DateRecorded == record.DateRecorded);
            }
        }

        [Fact]
        public async Task GetUserWeightRecordsAsync_InvalidUser_ReturnsNoRecords()
        {
            var user1 = new User { Username = "User1", Email = "user1@test.com", Password = "TestPassword" };
            var user2 = new User { Username = "User2", Email = "user2@test.com", Password = "TestPassword" };

            await _context.Users.AddRangeAsync(user1, user2);
            await _context.SaveChangesAsync();

            var service = new WeightService(_context);

            var weightRecord = new Weight { UserId = user1.Id, Value = 200, DateRecorded = DateTime.UtcNow };
            await _context.Weights.AddAsync(weightRecord);
            await _context.SaveChangesAsync();

            var results = await service.GetUserWeightsAsync(user2.Id);

            Assert.Empty(results);

        }

        [Fact]
        public async Task GetUserWeightRecordsAsync_NotExistentUser_ThrowsNotFoundError()
        {
            var nonExistentUserId = 999;
            var service = new WeightService(_context);

            await Assert.ThrowsAsync<NotFoundException>(() => service.GetUserWeightsAsync(nonExistentUserId));
        }
    }
}