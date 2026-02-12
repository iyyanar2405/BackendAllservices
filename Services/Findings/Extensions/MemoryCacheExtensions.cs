// Extensions/MemoryCacheExtensions.cs
using Microsoft.Extensions.Caching.Memory;

namespace FindingsAPI.Gateway.Extensions
{
    public static class MemoryCacheExtensions
    {
        public static IEnumerable<string> GetKeys(this IMemoryCache cache)
        {
            // This is a simplified implementation
            // In production, you might need a more sophisticated approach
            return new List<string>();
        }
    }
}