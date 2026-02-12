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
    /// User City Access management operations for city-level user permissions
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - User City Access")]
    public class UserCityAccessController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the UserCityAccessController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public UserCityAccessController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all user city access records with pagination and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="userId">Filter by user</param>
        /// <param name="cityId">Filter by city</param>
        /// <returns>Paginated list of user city access records</returns>
        /// <response code="200">Returns paginated list of user city access records</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<UserAccessResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetUserCityAccess(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? userId = null,
            [FromQuery] int? cityId = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.UserCityAccess
                .Include(u => u.User)
                .Include(u => u.City)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(u => u.UserId == userId.Value);

            if (cityId.HasValue)
                query = query.Where(u => u.CityId == cityId.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(u => u.User.LastName)
                .ThenBy(u => u.City.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserAccessResponseDto
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    EntityId = u.CityId,
                    EntityName = u.City.Name,
                    GrantedAt = u.GrantedAt,
                    ExpiresAt = null,
                    IsActive = u.User.IsActive && u.City.IsActive
                })
                .ToListAsync();

            var response = new PagedResponseDto<UserAccessResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<UserAccessResponseDto>>.CreateSuccess(response, "User city access records retrieved successfully"));
        }

        /// <summary>
        /// Get a specific user city access by ID
        /// </summary>
        /// <param name="id">User city access unique identifier</param>
        /// <returns>User city access details</returns>
        /// <response code="200">Returns user city access details</response>
        /// <response code="404">User city access not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserAccessResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetUserCityAccess([Required] int id)
        {
            var userAccess = await _context.UserCityAccess
                .Include(u => u.User)
                .Include(u => u.City)
                .Where(u => u.Id == id)
                .Select(u => new UserAccessResponseDto
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    EntityId = u.CityId,
                    EntityName = u.City.Name,
                    GrantedAt = u.GrantedAt,
                    ExpiresAt = null,
                    IsActive = u.User.IsActive && u.City.IsActive
                })
                .FirstOrDefaultAsync();

            if (userAccess == null)
                return NotFound(ApiResponse.CreateError("User city access not found", 404));

            return Ok(ApiResponse<UserAccessResponseDto>.CreateSuccess(userAccess, "User city access retrieved successfully"));
        }

        /// <summary>
        /// Create a new user city access
        /// </summary>
        /// <param name="model">User city access creation data</param>
        /// <returns>Created user city access details</returns>
        /// <response code="201">User city access created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">User or city not found</response>
        /// <response code="409">User city access already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserAccessResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateUserCityAccess([FromBody] CreateUserAccessDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var userExists = await _context.Users.AnyAsync(u => u.Id == model.UserId && u.IsActive);
            if (!userExists)
                return NotFound(ApiResponse.CreateError("User not found or inactive", 404));

            var cityExists = await _context.Cities.AnyAsync(c => c.Id == model.EntityId && c.IsActive);
            if (!cityExists)
                return NotFound(ApiResponse.CreateError("City not found or inactive", 404));

            var existing = await _context.UserCityAccess.FirstOrDefaultAsync(u => u.UserId == model.UserId && u.CityId == model.EntityId);
            if (existing != null)
                return Conflict(ApiResponse.CreateError("User city access already exists", 409));

            var userCityAccess = new UserCityAccess
            {
                UserId = model.UserId,
                CityId = model.EntityId,
                GrantedAt = DateTime.UtcNow
            };

            _context.UserCityAccess.Add(userCityAccess);
            await _context.SaveChangesAsync();

            await _context.Entry(userCityAccess).Reference(u => u.User).LoadAsync();
            await _context.Entry(userCityAccess).Reference(u => u.City).LoadAsync();

            var dto = new UserAccessResponseDto
            {
                Id = userCityAccess.Id,
                UserId = userCityAccess.UserId,
                UserName = $"{userCityAccess.User.FirstName} {userCityAccess.User.LastName}",
                EntityId = userCityAccess.CityId,
                EntityName = userCityAccess.City.Name,
                GrantedAt = userCityAccess.GrantedAt,
                ExpiresAt = null,
                IsActive = userCityAccess.User.IsActive && userCityAccess.City.IsActive
            };

            return CreatedAtAction(nameof(GetUserCityAccess), new { id = userCityAccess.Id },
                ApiResponse<UserAccessResponseDto>.CreateSuccess(dto, "User city access created successfully", 201));
        }

        /// <summary>
        /// Delete a user city access
        /// </summary>
        /// <param name="id">User city access unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">User city access deleted successfully</response>
        /// <response code="404">User city access not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteUserCityAccess([Required] int id)
        {
            var userCityAccess = await _context.UserCityAccess.FindAsync(id);
            if (userCityAccess == null)
                return NotFound(ApiResponse.CreateError("User city access not found", 404));

            _context.UserCityAccess.Remove(userCityAccess);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("User city access deleted successfully"));
        }

        /// <summary>
        /// Get cities accessible by user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of cities the user has access to</returns>
        /// <response code="200">Returns cities accessible by user</response>
        /// <response code="404">User not found</response>
        [HttpGet("user/{userId}/cities")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetUserCities([Required] int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            var cities = await _context.UserCityAccess
                .Include(u => u.City)
                .ThenInclude(c => c.Country)
                .Where(u => u.UserId == userId && u.City.IsActive)
                .Select(u => new
                {
                    Id = u.Id,
                    CityId = u.CityId,
                    CityName = u.City.Name,
                    CityCode = u.City.Code,
                    CountryName = u.City.Country.Name,
                    GrantedAt = u.GrantedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(cities, "User cities retrieved successfully"));
        }

        /// <summary>
        /// Get users with access to city
        /// </summary>
        /// <param name="cityId">City ID</param>
        /// <returns>List of users with access to the city</returns>
        /// <response code="200">Returns users with access to city</response>
        /// <response code="404">City not found</response>
        [HttpGet("city/{cityId}/users")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetCityUsers([Required] int cityId)
        {
            var cityExists = await _context.Cities.AnyAsync(c => c.Id == cityId);
            if (!cityExists)
                return NotFound(ApiResponse.CreateError("City not found", 404));

            var users = await _context.UserCityAccess
                .Include(u => u.User)
                .Where(u => u.CityId == cityId && u.User.IsActive)
                .Select(u => new
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    UserEmail = u.User.Email,
                    UserDepartment = u.User.Department,
                    GrantedAt = u.GrantedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(users, "City users retrieved successfully"));
        }

        /// <summary>
        /// Bulk assign cities to user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="cityIds">List of city IDs to assign</param>
        /// <returns>Bulk assignment confirmation</returns>
        /// <response code="200">Cities assigned successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">User not found or some cities not found</response>
        [HttpPost("user/{userId}/cities/bulk")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> BulkAssignCitiesToUser([Required] int userId, [FromBody] List<int> cityIds)
        {
            if (cityIds == null || !cityIds.Any())
                return BadRequest(ApiResponse.CreateError("City IDs list cannot be empty"));

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId && u.IsActive);
            if (!userExists)
                return NotFound(ApiResponse.CreateError("User not found or inactive", 404));

            var existingCities = await _context.Cities
                .Where(c => cityIds.Contains(c.Id) && c.IsActive)
                .Select(c => c.Id)
                .ToListAsync();

            if (existingCities.Count != cityIds.Count)
                return NotFound(ApiResponse.CreateError("Some cities not found or inactive", 404));

            // Get existing access
            var existingAccess = await _context.UserCityAccess
                .Where(u => u.UserId == userId && cityIds.Contains(u.CityId))
                .Select(u => u.CityId)
                .ToListAsync();

            // Only add new access
            var newCityIds = cityIds.Except(existingAccess).ToList();

            if (newCityIds.Any())
            {
                var newAccess = newCityIds.Select(cityId => new UserCityAccess
                {
                    UserId = userId,
                    CityId = cityId,
                    GrantedAt = DateTime.UtcNow
                });

                _context.UserCityAccess.AddRange(newAccess);
                await _context.SaveChangesAsync();
            }

            return Ok(ApiResponse.CreateSuccess($"Successfully assigned {newCityIds.Count} new cities to user. {existingAccess.Count} cities were already assigned."));
        }
    }
}