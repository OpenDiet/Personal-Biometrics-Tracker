using Microsoft.EntityFrameworkCore;
using PersonalBiometricsTracker.Data;
using PersonalBiometricsTracker.Dtos;
using PersonalBiometricsTracker.Entities;
using PersonalBiometricsTracker.Exceptions;
using PersonalBiometricsTracker.Services;

public class BloodGlucoseServiceTests
{
    [Fact]
    public async Task AddBloodGlucoseAsync_ValidData_AddsRecord()
    {
        var options = new DbContextOptionsBuilder<PersonalBiometricsTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_AddBloodGlucose")
            .Options;

        using (var context = new PersonalBiometricsTrackerDbContext(options))
        {
            var service = new BloodGlucoseService(context);
            var dto = new BloodGlucoseAddDto { Value = 5.6m, DateTimeRecorded = DateTime.Now };

            // Act
            var result = await service.AddBloodGlucoseAsync(1, dto);

            // Assert
            var addedEntry = await context.BloodGlucoses.FirstOrDefaultAsync(b => b.UserId == 1);
            Assert.NotNull(addedEntry);
            Assert.Equal(dto.Value, addedEntry.Value);
            Assert.Equal(dto.DateTimeRecorded.Value.Date, addedEntry.DateTimeRecorded.Date);
        }
    }
    [Fact]
    public async Task UpdateBloodGlucoseAsync_ValidData_UpdatesRecord()
    {
        var options = new DbContextOptionsBuilder<PersonalBiometricsTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_UpdateBloodGlucose")
            .Options;

        var userId = 1;
        var updatedValue = 7.8m;
        var updatedDateTime = DateTime.UtcNow;

        using (var context = new PersonalBiometricsTrackerDbContext(options))
        {
            var existingRecord = new BloodGlucose { UserId = userId, Value = 5.6m, DateTimeRecorded = DateTime.Now.AddDays(-1) };
            context.BloodGlucoses.Add(existingRecord);
            context.SaveChanges();

            var service = new BloodGlucoseService(context);
            var dto = new BloodGlucoseUpdateDto { Value = updatedValue, DateTimeRecorded = updatedDateTime };

            // Act
            await service.UpdateBloodGlucoseAsync(existingRecord.Id, userId, dto);

            // Assert
            var updatedEntry = await context.BloodGlucoses.FirstOrDefaultAsync(b => b.Id == existingRecord.Id);
            Assert.NotNull(updatedEntry);
            Assert.Equal(updatedValue, updatedEntry.Value);
            Assert.Equal(updatedDateTime, updatedEntry.DateTimeRecorded);
        }
    }

    [Fact]
    public async Task UpdateBloodGlucoseAsync_RecordNotFound_ThrowsNotFoundException()
    {
        var options = new DbContextOptionsBuilder<PersonalBiometricsTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_UpdateNotFound")
            .Options;

        using (var context = new PersonalBiometricsTrackerDbContext(options))
        {
            var service = new BloodGlucoseService(context);
            var dto = new BloodGlucoseUpdateDto { Value = 6.9m, DateTimeRecorded = DateTime.UtcNow };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateBloodGlucoseAsync(99, 1, dto));
        }
    }

    [Fact]
    public async Task AddBloodGlucoseAsync_InvalidData_Null_ThrowsValidationException()
    {
        var options = new DbContextOptionsBuilder<PersonalBiometricsTrackerDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDb_AddBloodGlucoseInvalidNull")
        .Options;

        using (var context = new PersonalBiometricsTrackerDbContext(options))
        {
            var service = new BloodGlucoseService(context);
            var dto = new BloodGlucoseAddDto { };

            await Assert.ThrowsAsync<ValidationException>(() => service.AddBloodGlucoseAsync(1, dto));
        }
    }

}
