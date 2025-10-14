using gus_API.Models.DTOs;
using gus_API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gus_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SellerController : ControllerBase
    {
        private readonly SellerService _sellerService;

        public SellerController(SellerService sellerService)
        {
            _sellerService = sellerService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterEp([FromBody] EntrepreneurDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _sellerService.RegisterEP(model);
                return Ok(new { message = "ИП успешно зарегистрирован. Ожидайте активации администратором." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }


        [HttpPost("toggle/{id}")]
        public async Task<IActionResult> ToggleEp(int id)
        {
            try
            {
                await _sellerService.EnterEp(id);
                return Ok(new { message = "Статус ИП изменен." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyEp()
        {
            try
            {
                var ep = await _sellerService.GetInfoEp();
                return Ok(ep);
            }
            catch (InvalidOperationException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var eps = await _sellerService.GetAll();
                return Ok(eps);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var ep = await _sellerService.GetInfoById(id);
                return Ok(ep);
            }
            catch(Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }

        }
    }
}
