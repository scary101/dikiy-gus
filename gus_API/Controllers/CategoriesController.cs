using gus_API.Models;
using gus_API.Models.DTOs.CategoryDTOs;
using gus_API.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql.PostgresTypes;
using System.Xml.Serialization;

namespace gus_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoriesController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddCategory([FromBody] CreateCategoryDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                await _categoryService.AddCategory(model);
                return Ok();


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var all = await _categoryService.GetAll();
                return Ok(all);

            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
        [HttpGet("getparent")]
        public async Task<IActionResult> GetParent()
        {
            try
            {
                var all = await _categoryService.GetCategories();
                return Ok(all);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
        [HttpGet("getsub/{id}")]
        public async Task<IActionResult> GetSub(int id)
        {
            try
            {
                var sub = await _categoryService.GetSubCategories(id);
                return Ok(sub);

            }catch (Exception ex) { return BadRequest(ex.Message); }
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> PutCategories([FromBody] CategoryDto model)
        {
            try
            {
                await _categoryService.RenameCategory(model);
                return Ok();
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
