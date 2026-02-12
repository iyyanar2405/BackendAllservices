// GraphQL/Middleware/GraphQLExecutionLogger.cs
using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;

namespace FindingsAPI.Gateway.GraphQL.Middleware
{
    public class GraphQLExecutionLogger : ExecutionDiagnosticEventListener
    {
        private readonly ILogger<GraphQLExecutionLogger> _logger;

        public GraphQLExecutionLogger(ILogger<GraphQLExecutionLogger> logger)
        {
            _logger = logger;
        }

        public override IDisposable ExecuteRequest(IRequestContext context)
        {
            var start = DateTime.UtcNow;
            
            return new RequestScope(_logger, context, start);
        }

        private class RequestScope : IDisposable
        {
            private readonly ILogger _logger;
            private readonly IRequestContext _context;
            private readonly DateTime _start;

            public RequestScope(ILogger logger, IRequestContext context, DateTime start)
            {
                _logger = logger;
                _context = context;
                _start = start;
            }

            public void Dispose()
            {
                var duration = DateTime.UtcNow - _start;
                
                _logger.LogInformation(
                    "GraphQL request completed: Duration: {Duration}ms",
                    duration.TotalMilliseconds);
            }
        }
    }
}