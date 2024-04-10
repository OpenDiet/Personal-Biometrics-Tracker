using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonalBiometricsTracker.Data;
using PersonalBiometricsTracker.Dtos;
using PersonalBiometricsTracker.Entities;
using PersonalBiometricsTracker.Exceptions;

namespace PersonalBiometricsTracker.Services
{
    public class UserService : IUserService
    {
        private readonly PersonalBiometricsTrackerDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public UserService(PersonalBiometricsTrackerDbContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<UserProfileDto> RegisterUserAsync(UserRegistrationDto userDto)
        {
            // Check if the user already exists
            var existingUserByEmail = await _context.Users.AnyAsync(u => u.Email == userDto.Email);
            var existingUserByUserName = await _context.Users.AnyAsync(u => u.Username == userDto.Username);

            if (existingUserByEmail)
            {
                throw new KeyAlreadyInUseException("Email address already in use.");
            }

            if (existingUserByUserName)
            {
                throw new KeyAlreadyInUseException("Username already in use.");
            }

            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
            };

            // Hash the password
            user.Password = _passwordHasher.HashPassword(user, userDto.Password);

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            var responseDto = new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            return responseDto;
        }

        public async Task<string> AuthenticateAsync(UserLoginDto userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userDto.Username);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            var PasswordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, userDto.Password);

            if (PasswordVerificationResult != PasswordVerificationResult.Success)
            {
                throw new InvalidCredentialsException("Invalid credentials");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return token;
        }

        public async Task<UserProfileDto> UpdateUserAsync(int userId, UserUpdateDto userUpdateDto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            // Update username if provided and it's different from the current one
            if (!string.IsNullOrEmpty(userUpdateDto.Username) && userUpdateDto.Username != user.Username)
            {
                var usernameExists = await _context.Users.AnyAsync(u => u.Username == userUpdateDto.Username);
                if (usernameExists)
                {
                    throw new KeyAlreadyInUseException("Username already in use.");
                }
                user.Username = userUpdateDto.Username;
            }

            // Update email if provided and it's different from the current one
            if (!string.IsNullOrEmpty(userUpdateDto.Email) && userUpdateDto.Email != user.Email)
            {
                var emailExists = await _context.Users.AnyAsync(u => u.Email == userUpdateDto.Email);
                if (emailExists)
                {
                    throw new KeyAlreadyInUseException("Email already in use.");
                }
                user.Email = userUpdateDto.Email;
            }

            // Update password if provided
            if (!string.IsNullOrEmpty(userUpdateDto.Password))
            {
                user.Password = _passwordHasher.HashPassword(user, userUpdateDto.Password);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new GenericException("An error occurred while trying to save the updated user. Database message: " + ex.Message);
            }

            var responseDto = new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            return responseDto;
        }


        private string GenerateJwtToken(User user)
        {
            var jwtSecret = _configuration.GetValue<string>("JWTSecret");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}