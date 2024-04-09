using PersonalBiometricsTracker.Dtos;
using PersonalBiometricsTracker.Entities;

namespace PersonalBiometricsTracker.Services
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(UserRegistrationDto userDto);
        Task<string> AuthenticateAsync(UserLoginDto userDto);
        Task<User> UpdateUserAsync(int userId, UserUpdateDto userUpdateDto);
    }
}