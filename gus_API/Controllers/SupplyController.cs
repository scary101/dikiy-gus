using gus_API.DTOs;
using gus_API.Models.DTOs.SupllyDTOs;
using gus_API.Models.DTOs.SupplyDTOs;
using gus_API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gus_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplyController : ControllerBase
    {
        private readonly SupplyService _supplyService;
        private readonly PdfService _pdfService;

        public SupplyController(SupplyService supplyService, PdfService pdfService)
        {
            _supplyService = supplyService;
            _pdfService = pdfService;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateSupply([FromBody] SupplyCreateDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Невалидные данные" });
                }

                await _supplyService.CreateSupplyAsync(model);
                return Ok(new { message = "Поставка успешно создана" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPut("status")]
        [Authorize]
        public async Task<IActionResult> UpdateSupplyStatus([FromBody] SupplyStatusDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Невалидные данные" });
                }

                await _supplyService.PutStatus(model);
                return Ok(new { message = "Статус поставки успешно обновлен" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAllSupplies()
        {
            try
            {
                var supplies = await _supplyService.GetAllSupplies();
                return Ok(supplies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("my-supplies")]
        [Authorize]
        public async Task<IActionResult> GetMySupplies()
        {
            try
            {
                var supplies = await _supplyService.GetSuppliesByEntrepreneur();
                return Ok(supplies);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }
        [HttpGet("{id}/document/pdf")]
        public async Task<IActionResult> GetSupplyPdf(int id)
        {
            var dto = await _supplyService.GetSupplyDocumentAsync(id);
            if (dto == null) return NotFound();

            var pdfBytes = _pdfService.GenerateSupplyPdf(dto);
            return File(pdfBytes, "application/pdf", $"Supply_{id}.pdf");
        }
    }
}