using ConsultantPlatform.Api.DTOs.Projects;

namespace ConsultantPlatform.Api.Interfaces
{
    public interface IProjectService
    {
        Task<List<ProjectResponseDto>> GetByCustomerAsync(int customerId, string? status = null);
        Task<ProjectResponseDto> CreateAsync(int customerId, CreateProjectDto dto, string userEmail);
        Task<ProjectResponseDto?> GetAsync(int id);
        Task<bool> UpdateAsync(int id, UpdateProjectDto dto, string userEmail);
        Task<bool> DeleteAsync(int id, string userEmail);
    }
}
