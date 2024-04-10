using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBiometricsTracker.Dtos;
using PersonalBiometricsTracker.Services;

namespace PersonalBiometricsTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeightController : ControllerBase
    {
        private readonly IWeightService _weightService;

        public WeightController(IWeightService weightService)
        {
            _weightService = weightService;
        }

        // POST: /Weight
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddWeight([FromBody] WeightAddDto weightAddDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                int userId = GetUserIdFromJwt();
                var weight = await _weightService.AddWeightAsync(weightAddDto, userId);
                return StatusCode(201, weight);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PATCH: /Weight/{id}
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateWeight(int id, [FromBody] WeightUpdateDto weightUpdateDto)
        {
            try
            {
                int userId = GetUserIdFromJwt();
                var updatedWeight = await _weightService.UpdateWeightAsync(id, userId, weightUpdateDto);
                return Ok(updatedWeight);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: /Weight/User/{userId}
        [HttpGet("User/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserWeights(int userId)
        {
            int userIdFromAuth = GetUserIdFromJwt();

            if (userId != userIdFromAuth)
            {
                return Unauthorized("You are not authorized to retrieve that user's weights.");
            }

            try
            {
                var weights = await _weightService.GetUserWeightsAsync(userId);
                return Ok(weights);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private int GetUserIdFromJwt()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim.Value);
        }

    }
}