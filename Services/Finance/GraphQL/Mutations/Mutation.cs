using FinanceService.Models;
using FinanceService.Services;

namespace FinanceService.GraphQL.Mutations
{
    public class Mutation
    {
        private readonly IFinanceService _service;

        public Mutation(IFinanceService service)
        {
            _service = service;
        }

        [GraphQLName("UpdatePlannedPaymentDate")]
        public Task<ApiResponse<bool>> UpdatePlannedPaymentDate(List<string> invoiceNumber, DateTime plannedDates)
        {
            return _service.UpdatePlannedPaymentDateAsync(invoiceNumber, plannedDates);
        }
    }
}
