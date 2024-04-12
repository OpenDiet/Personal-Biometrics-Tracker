using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PersonalBiometricsTracker.Data;
using PersonalBiometricsTracker.Dtos;
using PersonalBiometricsTracker.Entities;
using PersonalBiometricsTracker.Exceptions;
using PersonalBiometricsTracker.Services;

namespace PersonalBiometricsTracker.Tests
{

    public class UserServiceTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<PersonalBiometricsTrackerDbContext> _options;
        private readonly PersonalBiometricsTrackerDbContext _context;

        private readonly IConfiguration _configuration;

        public UserServiceTests()
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

            var config = new Dictionary<string, string>
            {
                ["JWTSecret"] = "this-is-a-test-jwt-secret-string"
            };

            _configuration = new ConfigurationBuilder().AddInMemoryCollection(config).Build();
        }

        public void Dispose()
        {
            // Dispose resources when tests are finished
            _context.Dispose();
            _connection.Close();
        }

        [Fact]
        public async Task RegisterUserAsync_ValidData_CreatesUser()
        {
            // Arrange
            var userDto = new UserRegistrationDto
            {
                Username = "NewUser",
                Email = "newuser@newuser.com",
                Password = "test-password"
            };

            var service = new UserService(_context, new PasswordHasher<User>(), _configuration);

            // Act
            var result = await service.RegisterUserAsync(userDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userDto.Username, result.Username);
            Assert.Equal(userDto.Email, result.Email);
            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == userDto.Username);

            Assert.NotNull(userInDb);
            Assert.Equal(userDto.Email, userInDb.Email);
        }

        [Fact]
        public async Task RegisterUserAsync_ExistingEmail_ThrowsKeyAlreadyInUseException()
        {
            // Arrange
            var existingUser = new User { Username = "existingUser", Email = "user@example.com", Password = "testpassword" };
            await _context.Users.AddAsync(existingUser);
            await _context.SaveChangesAsync();

            var newUser = new UserRegistrationDto { Username = "newUser", Email = "user@example.com", Password = "testpassword" };
            var service = new UserService(_context, new PasswordHasher<User>(), _configuration);

            // Act and Assert
            await Assert.ThrowsAsync<KeyAlreadyInUseException>(() => service.RegisterUserAsync(newUser));
        }

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var user = new User { Username = "validUser", Email = "validUser@example.com" };
            user.Password = new PasswordHasher<User>().HashPassword(user, "testpassword");
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var loginDto = new UserLoginDto { Username = "validUser", Password = "testpassword" };
            var service = new UserService(_context, new PasswordHasher<User>(), _configuration);

            // Act
            var token = await service.AuthenticateAsync(loginDto);

            // Assert
            Assert.NotNull(token);
        }

        [Fact]
        public async Task AuthenticateAsync_InvalidCredentials_ThrowsInvalidCredentialsException()
        {
            // Arrange
            var user = new User { Username = "validUser", Email = "validUser@example.com" };
            user.Password = new PasswordHasher<User>().HashPassword(user, "testpassword");
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var loginDto = new UserLoginDto { Username = "validUser", Password = "wrongPassword" };
            var service = new UserService(_context, new PasswordHasher<User>(), _configuration);

            // Act and Assert
            await Assert.ThrowsAsync<InvalidCredentialsException>(() => service.AuthenticateAsync(loginDto));
        }

        [Fact]
        public async Task UpdateUserAsync_ValidData_UpdatesUser()
        {
            // Arrange
            var user = new User { Username = "originalUser", Email = "original@example.com", Password = "originalPassword" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var updateDto = new UserUpdateDto { Username = "updatedUser", Email = "updatedEmail@example.com", Password = "updatedPassword" };
            var service = new UserService(_context, new PasswordHasher<User>(), _configuration);

            // Act
            var updatedProfile = await service.UpdateUserAsync(user.Id, updateDto);

            // Assert
            Assert.NotNull(updatedProfile);
            Assert.Equal(updateDto.Username, updatedProfile.Username);
            Assert.Equal(updateDto.Email, updatedProfile.Email);
            var userInDb = await _context.Users.FindAsync(user.Id);
            Assert.Equal(updateDto.Username, userInDb.Username);
            Assert.Equal(updateDto.Email, userInDb.Email);
            Assert.NotEqual("originalPassword", userInDb.Password);
        }
    }
}