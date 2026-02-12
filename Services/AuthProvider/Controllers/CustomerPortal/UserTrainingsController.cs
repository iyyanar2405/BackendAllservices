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
    /// User Training management operations for training enrollment and progress tracking
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - User Trainings")]
    public class UserTrainingsController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        public UserTrainingsController(CustomerPortalContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<UserTrainingResponseDto>>), 200)]
        public async Task<IActionResult> GetUserTrainings(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? userId = null,
            [FromQuery] int? trainingId = null,
            [FromQuery] TrainingStatus? status = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.UserTrainings
                .Include(u => u.User)
                .Include(u => u.Training)
                .AsQueryable();

            if (userId.HasValue) query = query.Where(u => u.UserId == userId.Value);
            if (trainingId.HasValue) query = query.Where(u => u.TrainingId == trainingId.Value);
            if (status.HasValue) query = query.Where(u => u.Status == status.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(u => u.EnrolledAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserTrainingResponseDto
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    TrainingId = u.TrainingId,
                    TrainingTitle = u.Training.Title,
                    EnrolledAt = u.EnrolledAt,
                    CompletedAt = u.CompletedAt,
                    Status = u.Status,
                    Progress = u.Progress,
                    Score = u.Score
                })
                .ToListAsync();

            var response = new PagedResponseDto<UserTrainingResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<UserTrainingResponseDto>>.CreateSuccess(response, "User trainings retrieved successfully"));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserTrainingResponseDto>), 200)]
        public async Task<IActionResult> GetUserTraining([Required] int id)
        {
            var userTraining = await _context.UserTrainings
                .Include(u => u.User)
                .Include(u => u.Training)
                .Where(u => u.Id == id)
                .Select(u => new UserTrainingResponseDto
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    TrainingId = u.TrainingId,
                    TrainingTitle = u.Training.Title,
                    EnrolledAt = u.EnrolledAt,
                    CompletedAt = u.CompletedAt,
                    Status = u.Status,
                    Progress = u.Progress,
                    Score = u.Score
                })
                .FirstOrDefaultAsync();

            if (userTraining == null)
                return NotFound(ApiResponse.CreateError("User training not found", 404));

            return Ok(ApiResponse<UserTrainingResponseDto>.CreateSuccess(userTraining, "User training retrieved successfully"));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserTrainingResponseDto>), 201)]
        public async Task<IActionResult> CreateUserTraining([FromBody] CreateUserTrainingDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var userExists = await _context.Users.AnyAsync(u => u.Id == model.UserId && u.IsActive);
            if (!userExists) return NotFound(ApiResponse.CreateError("User not found or inactive", 404));

            var trainingExists = await _context.Trainings.AnyAsync(t => t.Id == model.TrainingId && t.IsActive);
            if (!trainingExists) return NotFound(ApiResponse.CreateError("Training not found or inactive", 404));

            var existing = await _context.UserTrainings.FirstOrDefaultAsync(u => u.UserId == model.UserId && u.TrainingId == model.TrainingId);
            if (existing != null) return Conflict(ApiResponse.CreateError("User already enrolled in this training", 409));

            // Check training capacity
            var training = await _context.Trainings
                .Include(t => t.UserTrainings)
                .FirstOrDefaultAsync(t => t.Id == model.TrainingId);

            if (training?.MaxParticipants.HasValue == true)
            {
                var currentEnrollments = training.UserTrainings.Count(ut => ut.Status != TrainingStatus.Cancelled);
                if (currentEnrollments >= training.MaxParticipants.Value)
                    return BadRequest(ApiResponse.CreateError("Training has reached maximum capacity", 400));
            }

            var userTraining = new UserTraining
            {
                UserId = model.UserId,
                TrainingId = model.TrainingId,
                EnrolledAt = DateTime.UtcNow,
                Status = TrainingStatus.Enrolled,
                Progress = 0
            };

            _context.UserTrainings.Add(userTraining);
            await _context.SaveChangesAsync();

            await _context.Entry(userTraining).Reference(u => u.User).LoadAsync();
            await _context.Entry(userTraining).Reference(u => u.Training).LoadAsync();

            var dto = new UserTrainingResponseDto
            {
                Id = userTraining.Id,
                UserId = userTraining.UserId,
                UserName = $"{userTraining.User.FirstName} {userTraining.User.LastName}",
                TrainingId = userTraining.TrainingId,
                TrainingTitle = userTraining.Training.Title,
                EnrolledAt = userTraining.EnrolledAt,
                CompletedAt = userTraining.CompletedAt,
                Status = userTraining.Status,
                Progress = userTraining.Progress,
                Score = userTraining.Score
            };

            return CreatedAtAction(nameof(GetUserTraining), new { id = userTraining.Id },
                ApiResponse<UserTrainingResponseDto>.CreateSuccess(dto, "User training enrollment created successfully", 201));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserTrainingResponseDto>), 200)]
        public async Task<IActionResult> UpdateUserTraining([Required] int id, [FromBody] object model)
        {
            var userTraining = await _context.UserTrainings
                .Include(u => u.User)
                .Include(u => u.Training)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (userTraining == null)
                return NotFound(ApiResponse.CreateError("User training not found", 404));

            // Parse the model dynamically to handle various update scenarios
            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var updateData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            if (updateData?.ContainsKey("status") == true && Enum.TryParse<TrainingStatus>(updateData["status"].ToString(), out var status))
            {
                userTraining.Status = status;
                if (status == TrainingStatus.Completed && userTraining.CompletedAt == null)
                    userTraining.CompletedAt = DateTime.UtcNow;
            }

            if (updateData?.ContainsKey("progress") == true && decimal.TryParse(updateData["progress"].ToString(), out var progress))
            {
                userTraining.Progress = Math.Max(0, Math.Min(100, progress));
                if (progress >= 100 && userTraining.Status == TrainingStatus.InProgress)
                {
                    userTraining.Status = TrainingStatus.Completed;
                    userTraining.CompletedAt ??= DateTime.UtcNow;
                }
            }

            if (updateData?.ContainsKey("score") == true && decimal.TryParse(updateData["score"].ToString(), out var score))
                userTraining.Score = Math.Max(0, Math.Min(100, score));

            await _context.SaveChangesAsync();

            var dto = new UserTrainingResponseDto
            {
                Id = userTraining.Id,
                UserId = userTraining.UserId,
                UserName = $"{userTraining.User.FirstName} {userTraining.User.LastName}",
                TrainingId = userTraining.TrainingId,
                TrainingTitle = userTraining.Training.Title,
                EnrolledAt = userTraining.EnrolledAt,
                CompletedAt = userTraining.CompletedAt,
                Status = userTraining.Status,
                Progress = userTraining.Progress,
                Score = userTraining.Score
            };

            return Ok(ApiResponse<UserTrainingResponseDto>.CreateSuccess(dto, "User training updated successfully"));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> DeleteUserTraining([Required] int id)
        {
            var userTraining = await _context.UserTrainings.FindAsync(id);
            if (userTraining == null) return NotFound(ApiResponse.CreateError("User training not found", 404));

            _context.UserTrainings.Remove(userTraining);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("User training enrollment deleted successfully"));
        }

        [HttpGet("user/{userId}/trainings")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserTrainingResponseDto>>), 200)]
        public async Task<IActionResult> GetTrainingsByUser([Required] int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists) return NotFound(ApiResponse.CreateError("User not found", 404));

            var trainings = await _context.UserTrainings
                .Include(u => u.Training)
                .Where(u => u.UserId == userId)
                .OrderByDescending(u => u.EnrolledAt)
                .Select(u => new UserTrainingResponseDto
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    TrainingId = u.TrainingId,
                    TrainingTitle = u.Training.Title,
                    EnrolledAt = u.EnrolledAt,
                    CompletedAt = u.CompletedAt,
                    Status = u.Status,
                    Progress = u.Progress,
                    Score = u.Score
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<UserTrainingResponseDto>>.CreateSuccess(trainings, "User trainings retrieved successfully"));
        }

        [HttpGet("training/{trainingId}/users")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserTrainingResponseDto>>), 200)]
        public async Task<IActionResult> GetUsersByTraining([Required] int trainingId)
        {
            var trainingExists = await _context.Trainings.AnyAsync(t => t.Id == trainingId);
            if (!trainingExists) return NotFound(ApiResponse.CreateError("Training not found", 404));

            var users = await _context.UserTrainings
                .Include(u => u.User)
                .Include(u => u.Training)
                .Where(u => u.TrainingId == trainingId)
                .OrderBy(u => u.User.LastName)
                .Select(u => new UserTrainingResponseDto
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    TrainingId = u.TrainingId,
                    TrainingTitle = u.Training.Title,
                    EnrolledAt = u.EnrolledAt,
                    CompletedAt = u.CompletedAt,
                    Status = u.Status,
                    Progress = u.Progress,
                    Score = u.Score
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<UserTrainingResponseDto>>.CreateSuccess(users, "Training users retrieved successfully"));
        }

        [HttpPost("training/{trainingId}/users/bulk")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> BulkEnrollUsers([Required] int trainingId, [FromBody] List<int> userIds)
        {
            if (userIds == null || !userIds.Any())
                return BadRequest(ApiResponse.CreateError("User IDs list cannot be empty"));

            var trainingExists = await _context.Trainings
                .Include(t => t.UserTrainings)
                .FirstOrDefaultAsync(t => t.Id == trainingId && t.IsActive);
            
            if (trainingExists == null) return NotFound(ApiResponse.CreateError("Training not found or inactive", 404));

            var existingUsers = await _context.Users
                .Where(u => userIds.Contains(u.Id) && u.IsActive)
                .Select(u => u.Id)
                .ToListAsync();

            if (existingUsers.Count != userIds.Count)
                return NotFound(ApiResponse.CreateError("Some users not found or inactive", 404));

            // Check capacity
            var currentEnrollments = trainingExists.UserTrainings.Count(ut => ut.Status != TrainingStatus.Cancelled);
            if (trainingExists.MaxParticipants.HasValue && currentEnrollments + userIds.Count > trainingExists.MaxParticipants.Value)
                return BadRequest(ApiResponse.CreateError($"Training capacity exceeded. Available slots: {trainingExists.MaxParticipants.Value - currentEnrollments}", 400));

            var existingEnrollments = await _context.UserTrainings
                .Where(u => u.TrainingId == trainingId && userIds.Contains(u.UserId))
                .Select(u => u.UserId)
                .ToListAsync();

            var newUserIds = userIds.Except(existingEnrollments).ToList();

            if (newUserIds.Any())
            {
                var newEnrollments = newUserIds.Select(userId => new UserTraining
                {
                    UserId = userId,
                    TrainingId = trainingId,
                    EnrolledAt = DateTime.UtcNow,
                    Status = TrainingStatus.Enrolled,
                    Progress = 0
                });

                _context.UserTrainings.AddRange(newEnrollments);
                await _context.SaveChangesAsync();
            }

            return Ok(ApiResponse.CreateSuccess($"Successfully enrolled {newUserIds.Count} new users in training. {existingEnrollments.Count} users were already enrolled."));
        }
    }
}