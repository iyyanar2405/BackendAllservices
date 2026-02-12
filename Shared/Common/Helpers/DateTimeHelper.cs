namespace Shared.Common.Helpers;

public static class DateTimeHelper
{
    public static DateTime GetUtcNow() => DateTime.UtcNow;

    public static DateTime ConvertToUtc(DateTime dateTime) => dateTime.Kind == DateTimeKind.Utc
        ? dateTime
        : dateTime.ToUniversalTime();

    public static string FormatForApi(DateTime dateTime) => dateTime.ToString("O");
}

public static class StringHelper
{
    public static bool IsNullOrEmpty(string? value) => string.IsNullOrEmpty(value);

    public static string Truncate(string value, int maxLength)
    {
        return value?.Length > maxLength ? value.Substring(0, maxLength) + "..." : value ?? string.Empty;
    }

    public static string ToSlug(string value)
    {
        return value?.ToLower().Replace(" ", "-").Replace("--", "-") ?? string.Empty;
    }
}
