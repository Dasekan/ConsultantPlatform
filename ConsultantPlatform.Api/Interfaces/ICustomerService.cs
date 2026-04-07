using ConsultantPlatform.Api.Dtos;

namespace ConsultantPlatform.Api.Interfaces
{
    public interface ICustomerService
    {
        Task<List<CustomerDto>> GetAllAsync(string? search = null, string? sort = null, string? status = null);
        Task<CustomerDto?> GetByIdAsync(int id);
        Task<CustomerDto> CreateAsync(CreateCustomerDto dto, string userEmail);
        Task<bool> UpdateAsync(int id, UpdateCustomerDto dto, string userEmail);
        Task<bool> DeleteAsync(int id, string userEmail);
    }
}
