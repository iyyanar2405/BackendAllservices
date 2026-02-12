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
    /// Cities management operations
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Cities")]
    public class CitiesController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        public CitiesController(CustomerPortalContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<CityResponseDto>>), 200)]
        public async Task<IActionResult> GetCities(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int? countryId = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.Cities.Include(c => c.Country).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(c => c.Name.Contains(searchTerm) || (c.Code != null && c.Code.Contains(searchTerm)));

            if (isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive.Value);

            if (countryId.HasValue)
                query = query.Where(c => c.CountryId == countryId.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CityResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    CountryId = c.CountryId,
                    CountryName = c.Country.Name,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    SitesCount = c.Sites.Count(s => s.IsActive)
                })
                .ToListAsync();

            var response = new PagedResponseDto<CityResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<CityResponseDto>>.CreateSuccess(response, "Cities retrieved successfully"));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CityResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetCity([Required] int id)
        {
            var city = await _context.Cities
                .Include(c => c.Country)
                .Include(c => c.Sites)
                .Where(c => c.Id == id)
                .Select(c => new CityResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    CountryId = c.CountryId,
                    CountryName = c.Country.Name,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    SitesCount = c.Sites.Count(s => s.IsActive)
                })
                .FirstOrDefaultAsync();

            if (city == null)
                return NotFound(ApiResponse.CreateError("City not found", 404));

            return Ok(ApiResponse<CityResponseDto>.CreateSuccess(city, "City retrieved successfully"));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CityResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateCity([FromBody] CreateCityDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var countryExists = await _context.Countries.AnyAsync(c => c.Id == model.CountryId && c.IsActive);
            if (!countryExists)
                return NotFound(ApiResponse.CreateError("Country not found or inactive", 404));

            var existing = await _context.Cities.FirstOrDefaultAsync(c => c.Name == model.Name && c.CountryId == model.CountryId);
            if (existing != null)
                return Conflict(ApiResponse.CreateError("City with this name already exists in the country", 409));

            var city = new City
            {
                Name = model.Name,
                Code = model.Code,
                CountryId = model.CountryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            await _context.Entry(city).Reference(c => c.Country).LoadAsync();

            var dto = new CityResponseDto
            {
                Id = city.Id,
                Name = city.Name,
                Code = city.Code,
                CountryId = city.CountryId,
                CountryName = city.Country.Name,
                IsActive = city.IsActive,
                CreatedAt = city.CreatedAt,
                SitesCount = 0
            };

            return CreatedAtAction(nameof(GetCity), new { id = city.Id },
                ApiResponse<CityResponseDto>.CreateSuccess(dto, "City created successfully", 201));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CityResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> UpdateCity([Required] int id, [FromBody] UpdateCityDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var city = await _context.Cities.Include(c => c.Country).FirstOrDefaultAsync(c => c.Id == id);
            if (city == null)
                return NotFound(ApiResponse.CreateError("City not found", 404));

            var countryExists = await _context.Countries.AnyAsync(c => c.Id == model.CountryId && c.IsActive);
            if (!countryExists)
                return NotFound(ApiResponse.CreateError("Country not found or inactive", 404));

            if (city.Name != model.Name || city.CountryId != model.CountryId)
            {
                var existing = await _context.Cities.FirstOrDefaultAsync(c => c.Name == model.Name && c.CountryId == model.CountryId && c.Id != id);
                if (existing != null)
                    return Conflict(ApiResponse.CreateError("City name is already in use in this country", 409));
            }

            city.Name = model.Name;
            city.Code = model.Code;
            city.CountryId = model.CountryId;
            city.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            var dto = new CityResponseDto
            {
                Id = city.Id,
                Name = city.Name,
                Code = city.Code,
                CountryId = city.CountryId,
                CountryName = city.Country.Name,
                IsActive = city.IsActive,
                CreatedAt = city.CreatedAt,
                SitesCount = city.Sites.Count(s => s.IsActive)
            };

            return Ok(ApiResponse<CityResponseDto>.CreateSuccess(dto, "City updated successfully"));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteCity([Required] int id)
        {
            var city = await _context.Cities.Include(c => c.Sites).FirstOrDefaultAsync(c => c.Id == id);
            if (city == null)
                return NotFound(ApiResponse.CreateError("City not found", 404));

            if (city.Sites.Any())
                return BadRequest(ApiResponse.CreateError("Cannot delete city with existing sites"));

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("City deleted successfully"));
        }

        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateCityStatus([Required] int id, [FromQuery] bool isActive)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
                return NotFound(ApiResponse.CreateError("City not found", 404));

            city.IsActive = isActive;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess($"City {(isActive ? "activated" : "deactivated")} successfully"));
        }
    }
}