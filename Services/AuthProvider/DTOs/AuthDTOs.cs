using System.ComponentModel.DataAnnotations;

namespace AuthProvider.DTOs
{
    /// <summary>
    /// Authentication token response DTO
    /// </summary>
    public class TokenResponseDto
    {
        /// <summary>
        /// JWT access token
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
        public string? AccessToken { get; set; }

        /// <summary>
        /// Refresh token for obtaining new access tokens
        /// </summary>
        /// <example>550e8400-e29b-41d4-a716-446655440000</example>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Token type (typically "bearer")
        /// </summary>
        /// <example>bearer</example>
        public string TokenType { get; set; } = "bearer";

        /// <summary>
        /// Token expiration time in seconds
        /// </summary>
        /// <example>3600</example>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Indicates if the user's email is verified
        /// </summary>
        /// <example>true</example>
        public bool HasVerifiedEmail { get; set; }

        /// <summary>
        /// Indicates if two-factor authentication is enabled
        /// </summary>
        /// <example>true</example>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Two-factor authentication token (if TFA is required)
        /// </summary>
        /// <example>123456</example>
        public string? TfaToken { get; set; }

        /// <summary>
        /// Last 4 digits of phone number for TFA
        /// </summary>
        /// <example>1234</example>
        public string? Last4 { get; set; }

        /// <summary>
        /// Device code for trusted devices
        /// </summary>
        /// <example>DEVICE123456</example>
        public string? DeviceCode { get; set; }

        /// <summary>
        /// Password reset token (if password reset is required)
        /// </summary>
        /// <example>RESET123456</example>
        public string? ResetToken { get; set; }
    }

    /// <summary>
    /// Login request DTO
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Username (email address)
        /// </summary>
        /// <example>john.doe@example.com</example>
        [Required(ErrorMessage = "Username is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// User password
        /// </summary>
        /// <example>Password123!</example>
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Device code for trusted devices (optional)
        /// </summary>
        /// <example>DEVICE123456</example>
        public string? DeviceCode { get; set; }

        /// <summary>
        /// Remember this login
        /// </summary>
        /// <example>true</example>
        public bool RememberMe { get; set; } = false;
    }

    /// <summary>
    /// User registration request DTO
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// User email address
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
        /// User phone number (optional)
        /// </summary>
        /// <example>+1234567890</example>
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }
    }

    /// <summary>
    /// Password reset request DTO
    /// </summary>
    public class ForgotPasswordDto
    {
        /// <summary>
        /// User email address
        /// </summary>
        /// <example>john.doe@example.com</example>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// Reset password with token DTO
    /// </summary>
    public class ResetPasswordDto
    {
        /// <summary>
        /// Password reset token
        /// </summary>
        /// <example>RESET123456ABCDEF</example>
        [Required(ErrorMessage = "Reset token is required")]
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// New password
        /// </summary>
        /// <example>NewPassword123!</example>
        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// New password confirmation
        /// </summary>
        /// <example>NewPassword123!</example>
        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// Change password DTO
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// Current password
        /// </summary>
        /// <example>OldPassword123!</example>
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// New password
        /// </summary>
        /// <example>NewPassword123!</example>
        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// New password confirmation
        /// </summary>
        /// <example>NewPassword123!</example>
        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// Email confirmation DTO
    /// </summary>
    public class ConfirmEmailDto
    {
        /// <summary>
        /// User ID
        /// </summary>
        /// <example>550e8400-e29b-41d4-a716-446655440000</example>
        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Email confirmation token
        /// </summary>
        /// <example>EMAIL123456ABCDEF</example>
        [Required(ErrorMessage = "Confirmation token is required")]
        public string Token { get; set; } = string.Empty;
    }

    /// <summary>
    /// Two-factor authentication setup DTO
    /// </summary>
    public class TwoFactorSetupDto
    {
        /// <summary>
        /// QR code for authenticator app setup
        /// </summary>
        /// <example>data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg==</example>
        public string? QrCodeImageUrl { get; set; }

        /// <summary>
        /// Manual entry key for authenticator apps
        /// </summary>
        /// <example>JBSWY3DPEHPK3PXP</example>
        public string? ManualEntryKey { get; set; }

        /// <summary>
        /// Backup recovery codes
        /// </summary>
        public IEnumerable<string>? RecoveryCodes { get; set; }
    }

    /// <summary>
    /// Two-factor authentication verification DTO
    /// </summary>
    public class VerifyTwoFactorDto
    {
        /// <summary>
        /// Two-factor authentication code
        /// </summary>
        /// <example>123456</example>
        [Required(ErrorMessage = "TFA code is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "TFA code must be 6 digits")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Remember this device for future logins
        /// </summary>
        /// <example>true</example>
        public bool RememberDevice { get; set; } = false;
    }
}