using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthProvider.Context;
using AuthProvider.Models.CustomerPortal;
using AuthProvider.DTOs.CustomerPortal;
using AuthProvider.DTOs;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AuthProvider.Controllers.CustomerPortal
{
    /// <summary>
    /// User Preferences management operations for individual user preference settings
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - User Preferences")]
    public class UserPreferencesController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        public UserPreferencesController(CustomerPortalContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<UserPreferenceResponseDto>>), 200)]
        public async Task<IActionResult> GetUserPreferences(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? userId = null,
            [FromQuery] string? key = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.UserPreferences
                .Include(u => u.User)
                .AsQueryable();

            if (userId.HasValue) query = query.Where(u => u.UserId == userId.Value);
            if (!string.IsNullOrWhiteSpace(key)) query = query.Where(u => u.Key.Contains(key));

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(u => u.User.LastName)
                .ThenBy(u => u.Key)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserPreferenceResponseDto
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    Key = u.Key,
                    Value = u.Value,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync();

            var response = new PagedResponseDto<UserPreferenceResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<UserPreferenceResponseDto>>.CreateSuccess(response, "User preferences retrieved successfully"));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserPreferenceResponseDto>), 200)]
        public async Task<IActionResult> GetUserPreference([Required] int id)
        {
            var preference = await _context.UserPreferences
                .Include(u => u.User)
                .Where(u => u.Id == id)
                .Select(u => new UserPreferenceResponseDto
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    Key = u.Key,
                    Value = u.Value,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (preference == null)
                return NotFound(ApiResponse.CreateError("User preference not found", 404));

            return Ok(ApiResponse<UserPreferenceResponseDto>.CreateSuccess(preference, "User preference retrieved successfully"));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserPreferenceResponseDto>), 201)]
        public async Task<IActionResult> CreateUserPreference([FromBody] CreateUserPreferenceDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var userExists = await _context.Users.AnyAsync(u => u.Id == model.UserId && u.IsActive);
            if (!userExists) return NotFound(ApiResponse.CreateError("User not found or inactive", 404));

            var existing = await _context.UserPreferences.FirstOrDefaultAsync(u => u.UserId == model.UserId && u.Key == model.Key);
            if (existing != null) return Conflict(ApiResponse.CreateError("User preference with this key already exists", 409));

            var preference = new UserPreference
            {
                UserId = model.UserId,
                Key = model.Key,
                Value = model.Value,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.UserPreferences.Add(preference);
            await _context.SaveChangesAsync();

            await _context.Entry(preference).Reference(u => u.User).LoadAsync();

            var dto = new UserPreferenceResponseDto
            {
                Id = preference.Id,
                UserId = preference.UserId,
                UserName = $"{preference.User.FirstName} {preference.User.LastName}",
                Key = preference.Key,
                Value = preference.Value,
                CreatedAt = preference.CreatedAt,
                UpdatedAt = preference.UpdatedAt
            };

            return CreatedAtAction(nameof(GetUserPreference), new { id = preference.Id },
                ApiResponse<UserPreferenceResponseDto>.CreateSuccess(dto, "User preference created successfully", 201));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserPreferenceResponseDto>), 200)]
        public async Task<IActionResult> UpdateUserPreference([Required] int id, [FromBody] UpdateUserPreferenceDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var preference = await _context.UserPreferences
                .Include(u => u.User)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (preference == null)
                return NotFound(ApiResponse.CreateError("User preference not found", 404));

            preference.Value = model.Value;
            preference.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var dto = new UserPreferenceResponseDto
            {
                Id = preference.Id,
                UserId = preference.UserId,
                UserName = $"{preference.User.FirstName} {preference.User.LastName}",
                Key = preference.Key,
                Value = preference.Value,
                CreatedAt = preference.CreatedAt,
                UpdatedAt = preference.UpdatedAt
            };

            return Ok(ApiResponse<UserPreferenceResponseDto>.CreateSuccess(dto, "User preference updated successfully"));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> DeleteUserPreference([Required] int id)
        {
            var preference = await _context.UserPreferences.FindAsync(id);
            if (preference == null) return NotFound(ApiResponse.CreateError("User preference not found", 404));

            _context.UserPreferences.Remove(preference);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("User preference deleted successfully"));
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserPreferenceResponseDto>>), 200)]
        public async Task<IActionResult> GetUserPreferencesByUser([Required] int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists) return NotFound(ApiResponse.CreateError("User not found", 404));

            var preferences = await _context.UserPreferences
                .Include(u => u.User)
                .Where(u => u.UserId == userId)
                .OrderBy(u => u.Key)
                .Select(u => new UserPreferenceResponseDto
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    Key = u.Key,
                    Value = u.Value,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<UserPreferenceResponseDto>>.CreateSuccess(preferences, "User preferences retrieved successfully"));
        }

        [HttpGet("user/{userId}/preference/{key}")]
        [ProducesResponseType(typeof(ApiResponse<UserPreferenceResponseDto>), 200)]
        public async Task<IActionResult> GetUserPreferenceByKey([Required] int userId, [Required] string key)
        {
            var preference = await _context.UserPreferences
                .Include(u => u.User)
                .Where(u => u.UserId == userId && u.Key == key)
                .Select(u => new UserPreferenceResponseDto
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    Key = u.Key,
                    Value = u.Value,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (preference == null)
                return NotFound(ApiResponse.CreateError("User preference not found", 404));

            return Ok(ApiResponse<UserPreferenceResponseDto>.CreateSuccess(preference, "User preference retrieved successfully"));
        }

        [HttpPost("user/{userId}/preferences/bulk")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> BulkUpsertUserPreferences([Required] int userId, [FromBody] Dictionary<string, string?> preferences)
        {
            if (preferences == null || !preferences.Any())
                return BadRequest(ApiResponse.CreateError("Preferences dictionary cannot be empty"));

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId && u.IsActive);
            if (!userExists) return NotFound(ApiResponse.CreateError("User not found or inactive", 404));

            var existingPreferences = await _context.UserPreferences
                .Where(u => u.UserId == userId && preferences.Keys.Contains(u.Key))
                .ToListAsync();

            var created = 0;
            var updated = 0;

            foreach (var pref in preferences)
            {
                var existing = existingPreferences.FirstOrDefault(e => e.Key == pref.Key);
                if (existing != null)
                {
                    existing.Value = pref.Value;
                    existing.UpdatedAt = DateTime.UtcNow;
                    updated++;
                }
                else
                {
                    _context.UserPreferences.Add(new UserPreference
                    {
                        UserId = userId,
                        Key = pref.Key,
                        Value = pref.Value,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                    created++;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess($"Successfully processed {preferences.Count} preferences: {created} created, {updated} updated"));
        }
    }
}