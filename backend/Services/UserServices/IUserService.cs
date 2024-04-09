using PersonalBiometricsTracker.Dtos;
using PersonalBiometricsTracker.Entities;

namespace PersonalBiometricsTracker.Services
{
    public interface IUserService
    {
        Task<UserProfileDto> RegisterUserAsync(UserRegistrationDto userDto);
        Task<string> AuthenticateAsync(UserLoginDto userDto);
        Task<UserProfileDto> UpdateUserAsync(int userId, UserUpdateDto userUpdateDto);
    }
}