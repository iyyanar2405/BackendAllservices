// Exceptions/FindingExceptions.cs
namespace FindingsAPI.Gateway
{
    public class FindingNotFoundException : Exception
    {
        public FindingNotFoundException(int findingId)
            : base($"Finding with ID {findingId} was not found")
        {
            FindingId = findingId;
        }

        public int FindingId { get; }
    }

    public class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException(string message)
            : base(message)
        {
        }
    }
}