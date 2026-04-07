using ConsultantPlatform.Api.DTOs.Projects;
using ConsultantPlatform.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultantPlatform.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _service;

    public ProjectsController(IProjectService service)
    {
        _service = service;
    }

    // GET /api/customers/{customerId}/projects
    [HttpGet("customers/{customerId:int}/projects")]
    public async Task<ActionResult<List<ProjectResponseDto>>> GetByCustomer(
    int customerId,
    [FromQuery] string? status)
    {
        var list = await _service.GetByCustomerAsync(customerId, status);
        return Ok(list);
    }

    // GET /api/projects/{id}
    [HttpGet("projects/{id:int}")]
    public async Task<ActionResult<ProjectResponseDto>> Get(int id)
    {
        var p = await _service.GetAsync(id);
        if (p == null) return NotFound();
        return Ok(p);
    }

    // POST /api/customers/{customerId}/projects (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpPost("customers/{customerId:int}/projects")]
    public async Task<ActionResult<ProjectResponseDto>> Create(int customerId, [FromBody] CreateProjectDto dto)
    {
        try
        {
            var userEmail = User.Identity?.Name ?? "Unknown";
            var created = await _service.CreateAsync(customerId, dto, userEmail);
            return Ok(created);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT /api/projects/{id} (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpPut("projects/{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectDto dto)
    {
        try
        {
            var userEmail = User.Identity?.Name ?? "Unknown";
            var ok = await _service.UpdateAsync(id, dto, userEmail);
            if (!ok) return NotFound();
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE /api/projects/{id} (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpDelete("projects/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userEmail = User.Identity?.Name ?? "Unknown";
            var ok = await _service.DeleteAsync(id, userEmail);

            if (!ok)
                return NotFound();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}
