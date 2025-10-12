using gus_API.Models.DTOs.ProductDTOs;
using gus_API.Service;
using Microsoft.AspNetCore.Mvc;

namespace gus_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacteristicController : ControllerBase
    {
        private readonly CharacteristicService _service;

        public CharacteristicController(CharacteristicService service)
        {
            _service = service;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddCharacteristic([FromBody] CharacteristicDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _service.AddCharacter(model);
                return Ok(new { message = "Характеристика успешно добавлена" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPost("add-depend")]
        public async Task<IActionResult> AddDepend([FromBody] DependCharacterCategoryDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _service.AddDepend(model);
                return Ok(new { message = "Связь успешно создана" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var list = await _service.GetAll();
                return Ok(list);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Ошибка при получении данных" });
            }
        }

        [HttpGet("by-category/{id:int}")]
        public async Task<IActionResult> GetAllByCategory(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var list = await _service.GetAllByCategory(id);
                return Ok(list);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Ошибка при получении данных" });
            }
        }
    }
}
