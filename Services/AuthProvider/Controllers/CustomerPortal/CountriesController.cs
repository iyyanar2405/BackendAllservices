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
    /// Countries management operations
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Countries")]
    public class CountriesController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the CountriesController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public CountriesController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all countries with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for filtering countries by name or code</param>
        /// <param name="isActive">Filter by active status</param>
        /// <returns>Paginated list of countries</returns>
        /// <response code="200">Returns paginated list of countries</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<CountryResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetCountries(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            if (pageNumber < 1)
                return BadRequest(ApiResponse.CreateError("Page number must be greater than 0"));

            if (pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Page size must be between 1 and 100"));

            var query = _context.Countries
                .Include(c => c.Cities)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => c.Name.Contains(searchTerm) ||
                                       (c.Code != null && c.Code.Contains(searchTerm)) ||
                                       (c.ISOCode != null && c.ISOCode.Contains(searchTerm)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();
            var countries = await query
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CountryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    ISOCode = c.ISOCode,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    CitiesCount = c.Cities.Count(city => city.IsActive)
                })
                .ToListAsync();

            var pagedResponse = new PagedResponseDto<CountryResponseDto>
            {
                Items = countries,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(ApiResponse<PagedResponseDto<CountryResponseDto>>.CreateSuccess(pagedResponse, "Countries retrieved successfully"));
        }

        /// <summary>
        /// Get a specific country by ID
        /// </summary>
        /// <param name="id">Country unique identifier</param>
        /// <returns>Country details</returns>
        /// <response code="200">Returns country details</response>
        /// <response code="404">Country not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CountryResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetCountry([Required] int id)
        {
            var country = await _context.Countries
                .Include(c => c.Cities)
                .Where(c => c.Id == id)
                .Select(c => new CountryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    ISOCode = c.ISOCode,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    CitiesCount = c.Cities.Count(city => city.IsActive)
                })
                .FirstOrDefaultAsync();

            if (country == null)
                return NotFound(ApiResponse.CreateError("Country not found", 404));

            return Ok(ApiResponse<CountryResponseDto>.CreateSuccess(country, "Country retrieved successfully"));
        }

        /// <summary>
        /// Create a new country
        /// </summary>
        /// <param name="model">Country creation data</param>
        /// <returns>Created country details</returns>
        /// <response code="201">Country created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="409">Country with name or code already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CountryResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            // Check if country with name already exists
            var existingCountry = await _context.Countries
                .FirstOrDefaultAsync(c => c.Name == model.Name);
            if (existingCountry != null)
                return Conflict(ApiResponse.CreateError("Country with this name already exists", 409));

            // Check if country with code already exists (if code is provided)
            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                var existingByCode = await _context.Countries
                    .FirstOrDefaultAsync(c => c.Code == model.Code);
                if (existingByCode != null)
                    return Conflict(ApiResponse.CreateError("Country with this code already exists", 409));
            }

            // Check if country with ISO code already exists (if ISO code is provided)
            if (!string.IsNullOrWhiteSpace(model.ISOCode))
            {
                var existingByISOCode = await _context.Countries
                    .FirstOrDefaultAsync(c => c.ISOCode == model.ISOCode);
                if (existingByISOCode != null)
                    return Conflict(ApiResponse.CreateError("Country with this ISO code already exists", 409));
            }

            var country = new Country
            {
                Name = model.Name,
                Code = model.Code,
                ISOCode = model.ISOCode,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            var countryDto = new CountryResponseDto
            {
                Id = country.Id,
                Name = country.Name,
                Code = country.Code,
                ISOCode = country.ISOCode,
                IsActive = country.IsActive,
                CreatedAt = country.CreatedAt,
                CitiesCount = 0
            };

            return CreatedAtAction(nameof(GetCountry), new { id = country.Id },
                ApiResponse<CountryResponseDto>.CreateSuccess(countryDto, "Country created successfully", 201));
        }

        /// <summary>
        /// Update an existing country
        /// </summary>
        /// <param name="id">Country unique identifier</param>
        /// <param name="model">Country update data</param>
        /// <returns>Updated country details</returns>
        /// <response code="200">Country updated successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Country not found</response>
        /// <response code="409">Country name or code is already in use</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CountryResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> UpdateCountry([Required] int id, [FromBody] UpdateCountryDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            var country = await _context.Countries
                .Include(c => c.Cities)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (country == null)
                return NotFound(ApiResponse.CreateError("Country not found", 404));

            // Check if name is being changed and if new name already exists
            if (country.Name != model.Name)
            {
                var existingCountry = await _context.Countries
                    .FirstOrDefaultAsync(c => c.Name == model.Name && c.Id != id);
                if (existingCountry != null)
                    return Conflict(ApiResponse.CreateError("Country name is already in use", 409));
            }

            // Check if code is being changed and if new code already exists
            if (country.Code != model.Code && !string.IsNullOrWhiteSpace(model.Code))
            {
                var existingByCode = await _context.Countries
                    .FirstOrDefaultAsync(c => c.Code == model.Code && c.Id != id);
                if (existingByCode != null)
                    return Conflict(ApiResponse.CreateError("Country code is already in use", 409));
            }

            // Check if ISO code is being changed and if new ISO code already exists
            if (country.ISOCode != model.ISOCode && !string.IsNullOrWhiteSpace(model.ISOCode))
            {
                var existingByISOCode = await _context.Countries
                    .FirstOrDefaultAsync(c => c.ISOCode == model.ISOCode && c.Id != id);
                if (existingByISOCode != null)
                    return Conflict(ApiResponse.CreateError("Country ISO code is already in use", 409));
            }

            country.Name = model.Name;
            country.Code = model.Code;
            country.ISOCode = model.ISOCode;
            country.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            var countryDto = new CountryResponseDto
            {
                Id = country.Id,
                Name = country.Name,
                Code = country.Code,
                ISOCode = country.ISOCode,
                IsActive = country.IsActive,
                CreatedAt = country.CreatedAt,
                CitiesCount = country.Cities.Count(c => c.IsActive)
            };

            return Ok(ApiResponse<CountryResponseDto>.CreateSuccess(countryDto, "Country updated successfully"));
        }

        /// <summary>
        /// Delete a country
        /// </summary>
        /// <param name="id">Country unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Country deleted successfully</response>
        /// <response code="404">Country not found</response>
        /// <response code="400">Cannot delete country with existing cities</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteCountry([Required] int id)
        {
            var country = await _context.Countries
                .Include(c => c.Cities)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (country == null)
                return NotFound(ApiResponse.CreateError("Country not found", 404));

            // Check if country has cities
            if (country.Cities.Any())
                return BadRequest(ApiResponse.CreateError("Cannot delete country with existing cities. Please move or delete them first."));

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("Country deleted successfully"));
        }

        /// <summary>
        /// Activate or deactivate a country
        /// </summary>
        /// <param name="id">Country unique identifier</param>
        /// <param name="isActive">Active status</param>
        /// <returns>Status change confirmation</returns>
        /// <response code="200">Country status updated successfully</response>
        /// <response code="404">Country not found</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateCountryStatus([Required] int id, [FromQuery] bool isActive)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
                return NotFound(ApiResponse.CreateError("Country not found", 404));

            country.IsActive = isActive;
            await _context.SaveChangesAsync();

            var status = isActive ? "activated" : "deactivated";
            return Ok(ApiResponse.CreateSuccess($"Country {status} successfully"));
        }

        /// <summary>
        /// Get cities by country
        /// </summary>
        /// <param name="id">Country ID</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10)</param>
        /// <returns>Paginated list of cities in the country</returns>
        /// <response code="200">Returns cities in the country</response>
        /// <response code="404">Country not found</response>
        [HttpGet("{id}/cities")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<CityResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetCitiesByCountry([Required] int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var countryExists = await _context.Countries.AnyAsync(c => c.Id == id);
            if (!countryExists)
                return NotFound(ApiResponse.CreateError("Country not found", 404));

            if (pageNumber < 1)
                return BadRequest(ApiResponse.CreateError("Page number must be greater than 0"));

            if (pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Page size must be between 1 and 100"));

            var query = _context.Cities
                .Include(c => c.Country)
                .Include(c => c.Sites)
                .Where(c => c.CountryId == id);

            var totalCount = await query.CountAsync();
            var cities = await query
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

            var pagedResponse = new PagedResponseDto<CityResponseDto>
            {
                Items = cities,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(ApiResponse<PagedResponseDto<CityResponseDto>>.CreateSuccess(pagedResponse, "Cities retrieved successfully"));
        }

        /// <summary>
        /// Search countries by name or code
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching countries</returns>
        /// <response code="200">Returns matching countries</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CountryResponseDto>>), 200)]
        public async Task<IActionResult> SearchCountries([FromQuery][Required] string searchTerm)
        {
            var countries = await _context.Countries
                .Where(c => c.IsActive && (c.Name.Contains(searchTerm) ||
                           (c.Code != null && c.Code.Contains(searchTerm)) ||
                           (c.ISOCode != null && c.ISOCode.Contains(searchTerm))))
                .OrderBy(c => c.Name)
                .Select(c => new CountryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    ISOCode = c.ISOCode,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    CitiesCount = c.Cities.Count(city => city.IsActive)
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<CountryResponseDto>>.CreateSuccess(countries, "Countries search completed successfully"));
        }
    }
}