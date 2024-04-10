using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBiometricsTracker.Dtos;
using PersonalBiometricsTracker.Exceptions;
using PersonalBiometricsTracker.Services;

namespace PersonalBiometricsTracker.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class BloodGlucoseController : ControllerBase
    {
        private readonly IBloodGlucoseService _bloodGlucoseService;

        public BloodGlucoseController(IBloodGlucoseService bloodGlucoseService)
        {
            _bloodGlucoseService = bloodGlucoseService;
        }

        // POST: /BloodGlucose
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddBloodGlucose([FromBody] BloodGlucoseAddDto bloodGlucoseAddDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                int userId = GetUserIdFromJwt();
                var bloodGlucose = await _bloodGlucoseService.AddBloodGlucoseAsync(userId, bloodGlucoseAddDto);
                return StatusCode(201, bloodGlucose);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PATCH /BloodGlucose/{id}
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBloodGlucose(int id, [FromBody] BloodGlucoseUpdateDto bloodGlucoseUpdateDto)
        {
            try
            {
                int userId = GetUserIdFromJwt();
                var updatedBloodGlucose = await _bloodGlucoseService.UpdateBloodGlucoseAsync(id, userId, bloodGlucoseUpdateDto);
                return Ok(updatedBloodGlucose);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: /BloodGlucose/User/{userId}
        [HttpGet("User/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserBloodGlucoses(int userId)
        {
            try
            {
                int userIdFromAuth = GetUserIdFromJwt();

                if (userId != userIdFromAuth)
                {
                    return Unauthorized("You are not authorized to retrieve that user's BloodGlucoses.");
                }

                var bloodGlucoses = await _bloodGlucoseService.GetUserBloodGlucoseRecordsAsync(userId);
                return Ok(bloodGlucoses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private int GetUserIdFromJwt()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new InvalidCredentialsException("The user ID could not be obtained from the token.");
            }

            return int.Parse(userIdClaim.Value);
        }

    }
}