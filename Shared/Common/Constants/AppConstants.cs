namespace Shared.Common.Constants;

public static class AppConstants
{
    public const string DefaultCulture = "en-US";
    public const string ApiVersion = "v1";
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    public static class HttpHeaders
    {
        public const string CorrelationId = "X-Correlation-Id";
        public const string Authorization = "Authorization";
        public const string ContentType = "Content-Type";
    }

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string User = "User";
    }
}

public static class ErrorCodes
{
    public const string InvalidRequest = "INVALID_REQUEST";
    public const string NotFound = "NOT_FOUND";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string InternalError = "INTERNAL_ERROR";
}
