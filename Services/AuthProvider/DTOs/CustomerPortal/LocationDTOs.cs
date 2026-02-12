using System.ComponentModel.DataAnnotations;

namespace AuthProvider.DTOs.CustomerPortal
{
    // Country DTOs
    public class CountryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? ISOCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CitiesCount { get; set; }
    }

    public class CreateCountryDto
    {
        [Required(ErrorMessage = "Country name is required")]
        [StringLength(100, ErrorMessage = "Country name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(5, ErrorMessage = "Code cannot exceed 5 characters")]
        public string? Code { get; set; }

        [StringLength(3, ErrorMessage = "ISO code cannot exceed 3 characters")]
        public string? ISOCode { get; set; }
    }

    public class UpdateCountryDto
    {
        [Required(ErrorMessage = "Country name is required")]
        [StringLength(100, ErrorMessage = "Country name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(5, ErrorMessage = "Code cannot exceed 5 characters")]
        public string? Code { get; set; }

        [StringLength(3, ErrorMessage = "ISO code cannot exceed 3 characters")]
        public string? ISOCode { get; set; }

        public bool IsActive { get; set; }
    }

    // City DTOs
    public class CityResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SitesCount { get; set; }
    }

    public class CreateCityDto
    {
        [Required(ErrorMessage = "City name is required")]
        [StringLength(100, ErrorMessage = "City name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "Code cannot exceed 10 characters")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public int CountryId { get; set; }
    }

    public class UpdateCityDto
    {
        [Required(ErrorMessage = "City name is required")]
        [StringLength(100, ErrorMessage = "City name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "Code cannot exceed 10 characters")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public int CountryId { get; set; }

        public bool IsActive { get; set; }
    }

    // Company DTOs
    public class CompanyResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int SitesCount { get; set; }
        public int AuditsCount { get; set; }
    }

    public class CreateCompanyDto
    {
        [Required(ErrorMessage = "Company name is required")]
        [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
        public string? Code { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }

        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string? Email { get; set; }

        [StringLength(255, ErrorMessage = "Website cannot exceed 255 characters")]
        public string? Website { get; set; }
    }

    public class UpdateCompanyDto
    {
        [Required(ErrorMessage = "Company name is required")]
        [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
        public string? Code { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }

        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string? Email { get; set; }

        [StringLength(255, ErrorMessage = "Website cannot exceed 255 characters")]
        public string? Website { get; set; }

        public bool IsActive { get; set; }
    }

    // Site DTOs
    public class SiteResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Address { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int CityId { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int AuditsCount { get; set; }
    }

    public class CreateSiteDto
    {
        [Required(ErrorMessage = "Site name is required")]
        [StringLength(200, ErrorMessage = "Site name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
        public string? Code { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Company is required")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "City is required")]
        public int CityId { get; set; }

        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string? Email { get; set; }
    }

    public class UpdateSiteDto
    {
        [Required(ErrorMessage = "Site name is required")]
        [StringLength(200, ErrorMessage = "Site name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
        public string? Code { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Company is required")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "City is required")]
        public int CityId { get; set; }

        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string? Email { get; set; }

        public bool IsActive { get; set; }
    }

    // Service DTOs
    public class ServiceResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int AuditsCount { get; set; }
        public int UsersCount { get; set; }
    }

    public class CreateServiceDto
    {
        [Required(ErrorMessage = "Service name is required")]
        [StringLength(100, ErrorMessage = "Service name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
        public string? Code { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
    }

    public class UpdateServiceDto
    {
        [Required(ErrorMessage = "Service name is required")]
        [StringLength(100, ErrorMessage = "Service name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
        public string? Code { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}