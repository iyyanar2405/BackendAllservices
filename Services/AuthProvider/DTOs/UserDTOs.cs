using System.ComponentModel.DataAnnotations;

namespace AuthProvider.DTOs
{
    /// <summary>
    /// User information response DTO
    /// </summary>
    public class UserResponseDto
    {
        /// <summary>
        /// User unique identifier
        /// </summary>
        /// <example>550e8400-e29b-41d4-a716-446655440000</example>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// User email address
        /// </summary>
        /// <example>john.doe@example.com</example>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User phone number
        /// </summary>
        /// <example>+1234567890</example>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Indicates if email is confirmed
        /// </summary>
        /// <example>true</example>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Indicates if account lockout is enabled
        /// </summary>
        /// <example>false</example>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// Indicates if two-factor authentication is enabled
        /// </summary>
        /// <example>true</example>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Date and time when password was last changed
        /// </summary>
        /// <example>2025-09-20T10:30:00Z</example>
        public DateTime? LastPasswordChanged { get; set; }

        /// <summary>
        /// User roles
        /// </summary>
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }

    /// <summary>
    /// User creation request DTO
    /// </summary>
    public class CreateUserDto
    {
        /// <summary>
        /// User email address (will be used as username)
        /// </summary>
        /// <example>john.doe@example.com</example>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User password
        /// </summary>
        /// <example>Password123!</example>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Password confirmation
        /// </summary>
        /// <example>Password123!</example>
        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// User phone number
        /// </summary>
        /// <example>+1234567890</example>
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Set if email should be marked as confirmed
        /// </summary>
        /// <example>false</example>
        public bool EmailConfirmed { get; set; } = false;
    }

    /// <summary>
    /// User update request DTO
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// User email address
        /// </summary>
        /// <example>john.doe@example.com</example>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User phone number
        /// </summary>
        /// <example>+1234567890</example>
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Set if email should be marked as confirmed
        /// </summary>
        /// <example>true</example>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Set if account lockout should be enabled
        /// </summary>
        /// <example>false</example>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// Set if two-factor authentication should be enabled
        /// </summary>
        /// <example>true</example>
        public bool TwoFactorEnabled { get; set; }
    }

    /// <summary>
    /// Role information response DTO
    /// </summary>
    public class RoleResponseDto
    {
        /// <summary>
        /// Role unique identifier
        /// </summary>
        /// <example>550e8400-e29b-41d4-a716-446655440000</example>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Role name
        /// </summary>
        /// <example>Administrator</example>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Role description
        /// </summary>
        /// <example>System administrator with full access</example>
        public string? Description { get; set; }

        /// <summary>
        /// Number of users assigned to this role
        /// </summary>
        /// <example>5</example>
        public int UserCount { get; set; }
    }

    /// <summary>
    /// Role creation/update request DTO
    /// </summary>
    public class CreateRoleDto
    {
        /// <summary>
        /// Role name
        /// </summary>
        /// <example>Administrator</example>
        [Required(ErrorMessage = "Role name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 50 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Role description
        /// </summary>
        /// <example>System administrator with full access</example>
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? Description { get; set; }
    }

    /// <summary>
    /// User-Role assignment DTO
    /// </summary>
    public class UserRoleDto
    {
        /// <summary>
        /// User ID
        /// </summary>
        /// <example>550e8400-e29b-41d4-a716-446655440000</example>
        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Role ID
        /// </summary>
        /// <example>550e8400-e29b-41d4-a716-446655440001</example>
        [Required(ErrorMessage = "Role ID is required")]
        public string RoleId { get; set; } = string.Empty;
    }

    /// <summary>
    /// Paginated response DTO
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    public class PagedResponseDto<T>
    {
        /// <summary>
        /// List of items for the current page
        /// </summary>
        public IEnumerable<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        /// <example>1</example>
        public int PageNumber { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        /// <example>10</example>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        /// <example>100</example>
        public int TotalCount { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        /// <example>10</example>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        /// <summary>
        /// Indicates if there is a previous page
        /// </summary>
        /// <example>false</example>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Indicates if there is a next page
        /// </summary>
        /// <example>true</example>
        public bool HasNextPage => PageNumber < TotalPages;
    }
}