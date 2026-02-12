using System.ComponentModel.DataAnnotations;

namespace AuthProvider.DTOs
{
    /// <summary>
    /// Standard API response wrapper
    /// </summary>
    /// <typeparam name="T">The type of data being returned</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates if the operation was successful
        /// </summary>
        /// <example>true</example>
        public bool Success { get; set; }

        /// <summary>
        /// The response data
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Error message if operation failed
        /// </summary>
        /// <example>User not found</example>
        public string? Message { get; set; }

        /// <summary>
        /// List of validation errors
        /// </summary>
        public IEnumerable<string>? Errors { get; set; }

        /// <summary>
        /// HTTP status code
        /// </summary>
        /// <example>200</example>
        public int StatusCode { get; set; }

        /// <summary>
        /// Timestamp of the response
        /// </summary>
        /// <example>2025-09-20T10:30:00Z</example>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Creates a successful response
        /// </summary>
        public static ApiResponse<T> CreateSuccess(T data, string? message = null, int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Creates an error response
        /// </summary>
        public static ApiResponse<T> CreateError(string message, int statusCode = 400, IEnumerable<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors
            };
        }

        /// <summary>
        /// Creates an error response with validation errors
        /// </summary>
        public static ApiResponse<T> CreateValidationError(IEnumerable<string> errors, string message = "Validation failed")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = 400,
                Errors = errors
            };
        }
    }

    /// <summary>
    /// Non-generic API response for operations that don't return data
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        /// <summary>
        /// Creates a successful response without data
        /// </summary>
        public static ApiResponse CreateSuccess(string? message = null, int statusCode = 200)
        {
            return new ApiResponse
            {
                Success = true,
                Message = message,
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Creates an error response without data
        /// </summary>
        public new static ApiResponse CreateError(string message, int statusCode = 400, IEnumerable<string>? errors = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors
            };
        }
    }
}