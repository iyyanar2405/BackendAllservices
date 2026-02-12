using FinanceService.Models;
using FinanceService.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceService.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoicesController : ControllerBase
    {
        private readonly IFinanceService _service;

        public InvoicesController(IFinanceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<InvoiceListPageData>>> GetInvoices(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? companyFilter = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var response = await _service.GetInvoiceListAsync(pageNumber, pageSize, status, companyFilter, startDate, endDate);
            return Ok(response);
        }

        [HttpGet("download")]
        public async Task<ActionResult<ApiResponse<DownloadInvoiceResponse>>> DownloadInvoice(
            [FromQuery] List<string> invoiceNumber,
            [FromQuery] int? userId = null)
        {
            var response = await _service.DownloadInvoiceAsync(invoiceNumber, userId);
            return Ok(response);
        }

        [HttpPut("planned-payment-date")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdatePlannedPaymentDate([FromBody] PlannedPaymentDateRequest request)
        {
            var response = await _service.UpdatePlannedPaymentDateAsync(request.InvoiceNumbers, request.PlannedPaymentDate);
            return Ok(response);
        }
    }
}
