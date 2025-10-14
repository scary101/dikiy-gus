using gus_API.Models.DTOs.AdminDTOs;
using gus_API.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gus_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var accounts = await _accountService.GetAll();
                return Ok(accounts);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("createnew")]
        public async Task<IActionResult> CreateNew([FromBody] AdminRegisterDto model)
        {
            try
            {
                await _accountService.CreateNewUser(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("ban/{id}")]
        public async Task<IActionResult> BanUser(int id)
        {
            try
            {
                await _accountService.BanUser(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
