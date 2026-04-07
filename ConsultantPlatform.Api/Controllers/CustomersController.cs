using ConsultantPlatform.Api.Dtos;
using ConsultantPlatform.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultantPlatform.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomersController(ICustomerService service)
        {
            _service = service;
        }

        // GET: /api/customers
        [HttpGet]
        public async Task<ActionResult<List<CustomerDto>>> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? sort,
            [FromQuery] string? status)
        {
            var result = await _service.GetAllAsync(search, sort, status);
            return Ok(result);
        }

        // GET: /api/customers/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerDto>> GetById(int id)
        {
            var customer = await _service.GetByIdAsync(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        // POST: /api/customers
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerDto dto)
        {
            try
            {
                var userEmail = User.Identity?.Name ?? "Unknown";
                var created = await _service.CreateAsync(dto, userEmail);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: /api/customers/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDto dto)
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

        // DELETE: /api/customers/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userEmail = User.Identity?.Name ?? "Unknown";
            var ok = await _service.DeleteAsync(id, userEmail);
            if (!ok) return NotFound();

            return NoContent();
        }
    }
}
