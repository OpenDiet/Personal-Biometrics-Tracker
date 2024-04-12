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

        /**
            ADD BLOOD GLUCOSE TESTS
        */
        [Fact]
        public async Task AddBloodGlucoseAsync_ValidData_AddsRecord()
        {
            // Arrange
            var user = new User { Username = "TestUser", Email = "test@test.com", Password = "TestPassword" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var service = new BloodGlucoseService(_context);
            var dto = new BloodGlucoseAddDto { Value = 5.6m, DateTimeRecorded = DateTime.Now };

            // Act
            await service.AddBloodGlucoseAsync(user.Id, dto);

            // Assert
            var addedEntry = await _context.BloodGlucoses.FirstOrDefaultAsync(b => b.UserId == user.Id);
            Assert.NotNull(addedEntry);
            Assert.Equal(dto.Value, addedEntry.Value);
            Assert.Equal(dto.DateTimeRecorded.Value.Date, addedEntry.DateTimeRecorded.Date);
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

        /*
            UPDATE BLOOD GLUCOSE TESTS
        */
        [Fact]
        public async Task UpdateBloodGlucoseAsync_ValidData_UpdatesRecord()
        {
            // Arrange
            var user = new User { Username = "TestUser", Email = "test@test.com", Password = "TestPassword" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var updatedValue = 7.8m;
            var updatedDateTime = DateTime.UtcNow;

            var existingRecord = new BloodGlucose { UserId = user.Id, Value = 5.6m, DateTimeRecorded = DateTime.Now.AddDays(-1) };
            _context.BloodGlucoses.Add(existingRecord);
            await _context.SaveChangesAsync();

            var service = new BloodGlucoseService(_context);
            var dto = new BloodGlucoseUpdateDto { Value = updatedValue, DateTimeRecorded = updatedDateTime };

            // Act
            await service.UpdateBloodGlucoseAsync(existingRecord.Id, user.Id, dto);

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
        public async Task UpdateBloodGlucoseAsync_UpdateAnotherUsersRecords_ThrowsNotFoundException()
        {
            // Arrange
            var authedUser = new User { Username = "AuthedUser", Email = "auth@test.com", Password = "TestPassword:" };
            var notAuthedUser = new User { Username = "NotAuthedUser", Email = "notauth@test.com", Password = "TestPassword" };
            _context.Users.Add(authedUser);
            _context.Users.Add(notAuthedUser);
            await _context.SaveChangesAsync();

            var service = new BloodGlucoseService(_context);
            var addDto = new BloodGlucoseAddDto { Value = 6.9m, DateTimeRecorded = DateTime.UtcNow };
            var updateDto = new BloodGlucoseUpdateDto { Value = 6.9m, DateTimeRecorded = DateTime.UtcNow };

            var notAuthedUserBGEntry = await service.AddBloodGlucoseAsync(notAuthedUser.Id, addDto);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateBloodGlucoseAsync(notAuthedUserBGEntry.Id, authedUser.Id, updateDto));
        }

        [Fact]
        public async Task GetUserBloodGlucoseRecordsAsync_ValidUser_ReturnsCorrectRecords()
        {
            // Arrange
            var user = new User { Username = "User1", Email = "user1@test.com", Password = "TestPassword" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var service = new BloodGlucoseService(_context);

            var bloodGlucoseRecords = new List<BloodGlucose>
    {
        new BloodGlucose { UserId = user.Id, Value = 5.6m, DateTimeRecorded = DateTime.UtcNow.AddDays(-1) },
        new BloodGlucose { UserId = user.Id, Value = 4.8m, DateTimeRecorded = DateTime.UtcNow }
    };

            await _context.BloodGlucoses.AddRangeAsync(bloodGlucoseRecords);
            await _context.SaveChangesAsync();

            // Act
            var results = await service.GetUserBloodGlucoseRecordsAsync(user.Id);

            // Assert
            Assert.Equal(2, results.Count());
            foreach (var record in results)
            {
                Assert.Contains(bloodGlucoseRecords, bg => bg.Value == record.Value && bg.DateTimeRecorded == record.DateTimeRecorded && bg.UserId == record.UserId);
            }
        }

        [Fact]
        public async Task GetUserBloodGlucoseRecordsAsync_InvalidUser_ReturnsNoRecords()
        {
            // Arrange: Create two users and blood glucose records only for one of them
            var user1 = new User { Username = "User1", Email = "user1@test.com", Password = "TestPassword" };
            var user2 = new User { Username = "User2", Email = "user2@test.com", Password = "TestPassword" };

            await _context.Users.AddRangeAsync(user1, user2);
            await _context.SaveChangesAsync();

            var service = new BloodGlucoseService(_context);

            var bloodGlucoseRecord = new BloodGlucose { UserId = user1.Id, Value = 5.6m, DateTimeRecorded = DateTime.UtcNow.AddDays(-1) };
            await _context.BloodGlucoses.AddAsync(bloodGlucoseRecord);
            await _context.SaveChangesAsync();

            // Act: Attempt to retrieve user2's blood glucose records (should be empty)
            var results = await service.GetUserBloodGlucoseRecordsAsync(user2.Id);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public async Task GetUserBloodGlucoseRecordsAsync_NonExistentUser_ThrowsNotFoundException()
        {
            // Arrange
            var nonExistentUserId = 999; // Assuming this user ID does not exist in the database
            var service = new BloodGlucoseService(_context);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.GetUserBloodGlucoseRecordsAsync(nonExistentUserId));
        }

    }

}