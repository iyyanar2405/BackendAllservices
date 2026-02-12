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
    /// User Country Access management operations for country-level user permissions
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - User Country Access")]
    public class UserCountryAccessController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        public UserCountryAccessController(CustomerPortalContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<UserAccessResponseDto>>), 200)]
        public async Task<IActionResult> GetUserCountryAccess(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? userId = null,
            [FromQuery] int? countryId = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.UserCountryAccess
                .Include(u => u.User)
                .Include(u => u.Country)
                .AsQueryable();

            if (userId.HasValue) query = query.Where(u => u.UserId == userId.Value);
            if (countryId.HasValue) query = query.Where(u => u.CountryId == countryId.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(u => u.User.LastName)
                .ThenBy(u => u.Country.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserAccessResponseDto
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    EntityId = u.CountryId,
                    EntityName = u.Country.Name,
                    GrantedAt = u.GrantedAt,
                    ExpiresAt = null,
                    IsActive = u.User.IsActive && u.Country.IsActive
                })
                .ToListAsync();

            var response = new PagedResponseDto<UserAccessResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<UserAccessResponseDto>>.CreateSuccess(response, "User country access records retrieved successfully"));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserAccessResponseDto>), 200)]
        public async Task<IActionResult> GetUserCountryAccess([Required] int id)
        {
            var userAccess = await _context.UserCountryAccess
                .Include(u => u.User)
                .Include(u => u.Country)
                .Where(u => u.Id == id)
                .Select(u => new UserAccessResponseDto
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    EntityId = u.CountryId,
                    EntityName = u.Country.Name,
                    GrantedAt = u.GrantedAt,
                    ExpiresAt = null,
                    IsActive = u.User.IsActive && u.Country.IsActive
                })
                .FirstOrDefaultAsync();

            if (userAccess == null)
                return NotFound(ApiResponse.CreateError("User country access not found", 404));

            return Ok(ApiResponse<UserAccessResponseDto>.CreateSuccess(userAccess, "User country access retrieved successfully"));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserAccessResponseDto>), 201)]
        public async Task<IActionResult> CreateUserCountryAccess([FromBody] CreateUserAccessDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var userExists = await _context.Users.AnyAsync(u => u.Id == model.UserId && u.IsActive);
            if (!userExists) return NotFound(ApiResponse.CreateError("User not found or inactive", 404));

            var countryExists = await _context.Countries.AnyAsync(c => c.Id == model.EntityId && c.IsActive);
            if (!countryExists) return NotFound(ApiResponse.CreateError("Country not found or inactive", 404));

            var existing = await _context.UserCountryAccess.FirstOrDefaultAsync(u => u.UserId == model.UserId && u.CountryId == model.EntityId);
            if (existing != null) return Conflict(ApiResponse.CreateError("User country access already exists", 409));

            var userCountryAccess = new UserCountryAccess
            {
                UserId = model.UserId,
                CountryId = model.EntityId,
                GrantedAt = DateTime.UtcNow
            };

            _context.UserCountryAccess.Add(userCountryAccess);
            await _context.SaveChangesAsync();

            await _context.Entry(userCountryAccess).Reference(u => u.User).LoadAsync();
            await _context.Entry(userCountryAccess).Reference(u => u.Country).LoadAsync();

            var dto = new UserAccessResponseDto
            {
                Id = userCountryAccess.Id,
                UserId = userCountryAccess.UserId,
                UserName = $"{userCountryAccess.User.FirstName} {userCountryAccess.User.LastName}",
                EntityId = userCountryAccess.CountryId,
                EntityName = userCountryAccess.Country.Name,
                GrantedAt = userCountryAccess.GrantedAt,
                ExpiresAt = null,
                IsActive = userCountryAccess.User.IsActive && userCountryAccess.Country.IsActive
            };

            return CreatedAtAction(nameof(GetUserCountryAccess), new { id = userCountryAccess.Id },
                ApiResponse<UserAccessResponseDto>.CreateSuccess(dto, "User country access created successfully", 201));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> DeleteUserCountryAccess([Required] int id)
        {
            var userCountryAccess = await _context.UserCountryAccess.FindAsync(id);
            if (userCountryAccess == null) return NotFound(ApiResponse.CreateError("User country access not found", 404));

            _context.UserCountryAccess.Remove(userCountryAccess);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("User country access deleted successfully"));
        }

        [HttpGet("user/{userId}/countries")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), 200)]
        public async Task<IActionResult> GetUserCountries([Required] int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists) return NotFound(ApiResponse.CreateError("User not found", 404));

            var countries = await _context.UserCountryAccess
                .Include(u => u.Country)
                .Where(u => u.UserId == userId && u.Country.IsActive)
                .Select(u => new { u.Id, u.CountryId, u.Country.Name, u.Country.Code, u.GrantedAt })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(countries, "User countries retrieved successfully"));
        }

        [HttpGet("country/{countryId}/users")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), 200)]
        public async Task<IActionResult> GetCountryUsers([Required] int countryId)
        {
            var countryExists = await _context.Countries.AnyAsync(c => c.Id == countryId);
            if (!countryExists) return NotFound(ApiResponse.CreateError("Country not found", 404));

            var users = await _context.UserCountryAccess
                .Include(u => u.User)
                .Where(u => u.CountryId == countryId && u.User.IsActive)
                .Select(u => new { u.Id, u.UserId, UserName = $"{u.User.FirstName} {u.User.LastName}", u.User.Email, u.GrantedAt })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(users, "Country users retrieved successfully"));
        }
    }
}