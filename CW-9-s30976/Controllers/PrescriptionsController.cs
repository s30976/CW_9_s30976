using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CW_9_s30976.DTOs;
using CW_9_s30976.Services;
namespace CW_9_s30976.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrescriptionsController : ControllerBase
{
    private readonly IPrescriptionService _service;

    public PrescriptionsController(IPrescriptionService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] PrescriptionCreateDto dto)
    {
        try
        {
            await _service.AddPrescriptionAsync(dto);
            return Ok("Prescription created.");
        }
        catch (DbUpdateException dbu)
        {
            return BadRequest(dbu.InnerException?.Message ?? dbu.Message);
        }
        catch (ArgumentException aex)
        {
            return BadRequest(aex.Message);
        }
    }

    [HttpGet("patient/{id}")]
    public async Task<IActionResult> GetPatientDetails(int id)
    {
        var result = await _service.GetPatientDataAsync(id);

        if (result == null)
            return NotFound($"Patient with id {id} not found.");

        return Ok(result);
    }
}