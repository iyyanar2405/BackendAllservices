namespace ContractService.Models
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }
    }
}
