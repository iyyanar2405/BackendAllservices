using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AuthProvider.ViewModel;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthProvider.Models;
using System.Security.Claims;
using AuthProvider.Configuration;
using AuthProvider.DTOs;
using System.ComponentModel.DataAnnotations;

namespace AuthProvider.Controllers
{
    /// <summary>
    /// Claims management operations for users and roles
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("Claims Management")]
    public class ClaimsController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthProviderClaims _authClaims;

        /// <summary>
        /// Initializes a new instance of the ClaimsController
        /// </summary>
        /// <param name="roleManager">Role manager service</param>
        /// <param name="userManager">User manager service</param>
        public ClaimsController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this._roleManager = roleManager;
            this._userManager = userManager;
            this._authClaims = new AuthProviderClaims();
        }

        /// <summary>
        /// Get all claims for the current user
        /// </summary>
        /// <returns>List of current user's claims</returns>
        /// <response code="200">Returns list of user claims</response>
        /// <response code="401">User not authenticated</response>
        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<KeyValuePair<string, string>>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        public IActionResult GetCurrentUserClaims()
        {
            var userClaimsList = User.Identities.FirstOrDefault();
            if (userClaimsList == null)
                return Unauthorized(ApiResponse.CreateError("User not authenticated", 401));

            var claims = new List<KeyValuePair<string, string>>();
            foreach (var claim in userClaimsList.Claims)
            {
                claims.Add(new KeyValuePair<string, string>(claim.Type, claim.Value));
            }

            return Ok(ApiResponse<IEnumerable<KeyValuePair<string, string>>>.CreateSuccess(claims, "User claims retrieved successfully"));
        }

        /// <summary>
        /// Get all claims for a specific user
        /// </summary>
        /// <param name="userId">User unique identifier</param>
        /// <returns>List of user's claims</returns>
        /// <response code="200">Returns list of user claims</response>
        /// <response code="400">Invalid user ID</response>
        /// <response code="404">User not found</response>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<KeyValuePair<string, string>>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetUserClaims([Required] string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(ApiResponse.CreateError("User ID is required"));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            var userClaims = await _userManager.GetClaimsAsync(user);
            var claims = userClaims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value)).ToList();

            return Ok(ApiResponse<IEnumerable<KeyValuePair<string, string>>>.CreateSuccess(claims, $"Claims for user '{user.Email}' retrieved successfully"));
        }

        /// <summary>
        /// Get all claims for a specific role
        /// </summary>
        /// <param name="roleId">Role unique identifier</param>
        /// <returns>List of role's claims</returns>
        /// <response code="200">Returns list of role claims</response>
        /// <response code="400">Invalid role ID</response>
        /// <response code="404">Role not found</response>
        [HttpGet("role/{roleId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<KeyValuePair<string, string>>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetRoleClaims([Required] string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
                return BadRequest(ApiResponse.CreateError("Role ID is required"));

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound(ApiResponse.CreateError("Role not found", 404));

            var roleClaims = await _roleManager.GetClaimsAsync(role);
            var claims = roleClaims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value)).ToList();

            return Ok(ApiResponse<IEnumerable<KeyValuePair<string, string>>>.CreateSuccess(claims, $"Claims for role '{role.Name}' retrieved successfully"));
        }

        /// <summary>
        /// Get all available claim types
        /// </summary>
        /// <returns>List of valid claim types</returns>
        /// <response code="200">Returns list of available claim types</response>
        [HttpGet("types")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<string>>), 200)]
        public IActionResult GetAvailableClaimTypes()
        {
            var claimTypes = _authClaims.ListAllClaims();
            return Ok(ApiResponse<IEnumerable<string>>.CreateSuccess(claimTypes, "Available claim types retrieved successfully"));
        }

        /// <summary>
        /// Add a claim to a user or role
        /// </summary>
        /// <param name="model">Claim creation data</param>
        /// <returns>Claim addition confirmation</returns>
        /// <response code="200">Claim added successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">User or role not found</response>
        /// <response code="409">Claim already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> AddClaim([FromBody] ClaimViewModel model)
        {
            if (model == null)
                return BadRequest(ApiResponse.CreateError("No data in model"));

            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(validationErrors));
            }

            bool isUser = !string.IsNullOrEmpty(model.UserName);
            bool isRole = !string.IsNullOrEmpty(model.RoleName);

            if (!isUser && !isRole)
                return BadRequest(ApiResponse.CreateError("Must provide either UserName or RoleName"));

            // Validate claim type
            if (!string.IsNullOrEmpty(model.ClaimType) && !_authClaims.ValidateClaimType(model.ClaimType))
            {
                return BadRequest(ApiResponse.CreateError($"Invalid claim type. Valid types are: {string.Join(", ", _authClaims.ListAllClaims())}"));
            }

            var claim = new Claim(model.ClaimType, model.ClaimValue);
            var results = new List<IdentityResult>();
            var errorList = new List<string>();

            // Handle user claim
            if (isUser)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                    return NotFound(ApiResponse.CreateError("User not found", 404));

                var userClaims = await _userManager.GetClaimsAsync(user);
                if (userClaims.Any(c => c.Type == model.ClaimType && c.Value.Equals(model.ClaimValue, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return Conflict(ApiResponse.CreateError("User claim already exists", 409));
                }

                var result = await _userManager.AddClaimAsync(user, claim);
                results.Add(result);
                
                if (!result.Succeeded)
                    errorList.AddRange(result.Errors.Select(e => e.Description));
            }

            // Handle role claim
            if (isRole)
            {
                var role = await _roleManager.FindByNameAsync(model.RoleName);
                if (role == null)
                    return NotFound(ApiResponse.CreateError("Role not found", 404));

                var roleClaims = await _roleManager.GetClaimsAsync(role);
                if (roleClaims.Any(c => c.Type == model.ClaimType && c.Value.Equals(model.ClaimValue, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return Conflict(ApiResponse.CreateError("Role claim already exists", 409));
                }

                var result = await _roleManager.AddClaimAsync(role, claim);
                results.Add(result);
                
                if (!result.Succeeded)
                    errorList.AddRange(result.Errors.Select(e => e.Description));
            }

            if (results.Any(r => r.Succeeded))
            {
                return Ok(ApiResponse.CreateSuccess("Claim added successfully"));
            }

            return BadRequest(ApiResponse.CreateValidationError(errorList, "Failed to add claim"));
        }

        /// <summary>
        /// Update a user claim
        /// </summary>
        /// <param name="model">Claim update data</param>
        /// <returns>Claim update confirmation</returns>
        /// <response code="200">Claim updated successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">User or original claim not found</response>
        [HttpPut("user")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateUserClaim([FromBody] ClaimUpdateViewModel model)
        {
            if (model == null)
                return BadRequest(ApiResponse.CreateError("No data in model"));

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            if (!string.IsNullOrEmpty(model.ClaimType) && !_authClaims.ValidateClaimType(model.ClaimType))
            {
                return BadRequest(ApiResponse.CreateError($"Invalid claim type. Valid types are: {string.Join(", ", _authClaims.ListAllClaims())}"));
            }

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            var userClaims = await _userManager.GetClaimsAsync(user);
            var originalClaim = userClaims.FirstOrDefault(c => c.Type == model.OriginalClaimType && c.Value == model.OriginalClaimValue);
            
            if (originalClaim == null)
                return NotFound(ApiResponse.CreateError("Original claim not found", 404));

            var newClaim = new Claim(model.ClaimType, model.ClaimValue);
            var result = await _userManager.ReplaceClaimAsync(user, originalClaim, newClaim);

            if (result.Succeeded)
                return Ok(ApiResponse.CreateSuccess("User claim updated successfully"));

            var identityErrors = result.Errors.Select(e => e.Description);
            return BadRequest(ApiResponse.CreateValidationError(identityErrors, "Failed to update user claim"));
        }

        /// <summary>
        /// Update a role claim
        /// </summary>
        /// <param name="model">Claim update data</param>
        /// <returns>Claim update confirmation</returns>
        /// <response code="200">Claim updated successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Role or original claim not found</response>
        [HttpPut("role")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateRoleClaim([FromBody] ClaimUpdateViewModel model)
        {
            if (model == null)
                return BadRequest(ApiResponse.CreateError("No data in model"));

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            if (!string.IsNullOrEmpty(model.ClaimType) && !_authClaims.ValidateClaimType(model.ClaimType))
            {
                return BadRequest(ApiResponse.CreateError($"Invalid claim type. Valid types are: {string.Join(", ", _authClaims.ListAllClaims())}"));
            }

            var role = await _roleManager.FindByNameAsync(model.RoleName);
            if (role == null)
                return NotFound(ApiResponse.CreateError("Role not found", 404));

            var roleClaims = await _roleManager.GetClaimsAsync(role);
            var originalClaim = roleClaims.FirstOrDefault(c => c.Type == model.OriginalClaimType && c.Value == model.OriginalClaimValue);
            
            if (originalClaim == null)
                return NotFound(ApiResponse.CreateError("Original claim not found", 404));

            // Remove old claim and add new one
            await _roleManager.RemoveClaimAsync(role, originalClaim);
            
            var newClaim = new Claim(model.ClaimType, model.ClaimValue);
            var result = await _roleManager.AddClaimAsync(role, newClaim);

            if (result.Succeeded)
                return Ok(ApiResponse.CreateSuccess("Role claim updated successfully"));

            var identityErrors = result.Errors.Select(e => e.Description);
            return BadRequest(ApiResponse.CreateValidationError(identityErrors, "Failed to update role claim"));
        }

        /// <summary>
        /// Remove a claim from a user
        /// </summary>
        /// <param name="model">Claim removal data</param>
        /// <returns>Claim removal confirmation</returns>
        /// <response code="200">Claim removed successfully</response>
        /// <response code="400">Invalid data</response>
        /// <response code="404">User or claim not found</response>
        [HttpDelete("user")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> RemoveUserClaim([FromBody] ClaimUpdateViewModel model)
        {
            if (model == null)
                return BadRequest(ApiResponse.CreateError("No data in model"));

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            var userClaims = await _userManager.GetClaimsAsync(user);
            var claimToRemove = userClaims.FirstOrDefault(c => c.Type == model.OriginalClaimType && c.Value == model.OriginalClaimValue);
            
            if (claimToRemove == null)
                return NotFound(ApiResponse.CreateError("Claim not found", 404));

            var result = await _userManager.RemoveClaimAsync(user, claimToRemove);

            if (result.Succeeded)
                return Ok(ApiResponse.CreateSuccess("User claim removed successfully"));

            var identityErrors = result.Errors.Select(e => e.Description);
            return BadRequest(ApiResponse.CreateValidationError(identityErrors, "Failed to remove user claim"));
        }

        /// <summary>
        /// Remove a claim from a role
        /// </summary>
        /// <param name="model">Claim removal data</param>
        /// <returns>Claim removal confirmation</returns>
        /// <response code="200">Claim removed successfully</response>
        /// <response code="400">Invalid data</response>
        /// <response code="404">Role or claim not found</response>
        [HttpDelete("role")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> RemoveRoleClaim([FromBody] ClaimViewModel model)
        {
            if (model == null)
                return BadRequest(ApiResponse.CreateError("No data in model"));

            var role = await _roleManager.FindByNameAsync(model.RoleName);
            if (role == null)
                return NotFound(ApiResponse.CreateError("Role not found", 404));

            var roleClaims = await _roleManager.GetClaimsAsync(role);
            var claimToRemove = roleClaims.FirstOrDefault(c => c.Type == model.ClaimType && c.Value == model.ClaimValue);
            
            if (claimToRemove == null)
                return NotFound(ApiResponse.CreateError("Claim not found", 404));

            var result = await _roleManager.RemoveClaimAsync(role, claimToRemove);

            if (result.Succeeded)
                return Ok(ApiResponse.CreateSuccess("Role claim removed successfully"));

            var identityErrors = result.Errors.Select(e => e.Description);
            return BadRequest(ApiResponse.CreateValidationError(identityErrors, "Failed to remove role claim"));
        }
    }
}