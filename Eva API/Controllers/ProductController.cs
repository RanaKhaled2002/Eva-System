using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using DocumentFormat.OpenXml.Wordprocessing;
using Eva_BLL.Interfaces;
using Eva_BLL.Repositories;
using Eva_DAL.DTOs;
using Eva_DAL.DTOs.Category;
using Eva_DAL.DTOs.Ingredient;
using Eva_DAL.DTOs.Product;
using Eva_DAL.Models;
using GTranslate;
using GTranslate.Translators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eva_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] ProductCreateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = await _productService.AddProduct(dto);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("GetAllProduct")]
        public async Task<IActionResult> GetAllProduct([FromQuery]PaginationParams pagination)
        {
            try
            {
                var result = await _productService.GetAllProducts(pagination);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetProductById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var result = await _productService.GetProductById(id);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("TranslateProduct/{ProductId}")]
        public async Task<IActionResult> TranslateProduct(int ProductId)
        {
            try
            {
                var result = await _productService.TranslateProduct(ProductId);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("TranslateProduct/{ProductId}/{LanguageCode}")]
        public async Task<IActionResult> TranslateProductForSpecificLanguage(int ProductId, string LanguageCode)
        {
            try
            {
                var result = await _productService.TransalteProductForSpecificLanguage(ProductId,LanguageCode);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }

        }

        [HttpPut("UpdateSpecificTranslationFromSpecificIngredient/{ProductId}/{LanguageCode}")]
        public async Task<IActionResult> UpdateProductForSpecificLanguage(int ProductId, string LanguageCode, [FromBody] UpdateProductTranslationDto updatedTranslation)
        {
            try
            {
                var result = await _productService.UpdateTransalteProductForSpecificLanguage(ProductId,LanguageCode,updatedTranslation);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("DeleteSpecificTranslationFromSpecificProduct/{ProductId}/{LanguageCode}")]
        public async Task<IActionResult> DeleteProductForSpecificLanguage(int ProductId, string LanguageCode)
        {
            try
            {
                var result = await _productService.DeleteProductForSpecificLanguage(ProductId,LanguageCode);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetAllProductsBySpecificLanguage/{searchLanguage}")]
        public async Task<IActionResult> GetAllProductsBySpecificLanguage(string searchLanguage)
        {
            try
            {
                var result = await _productService.GetAllProductsBySpecificLanguage(searchLanguage);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetProductByIdAndSpecificLanguage/{productId}/{searchLanguage}")]
        public async Task<IActionResult> GetProductByIdAndLanguage(int productId, string searchLanguage)
        {
            try
            {
                var result = await _productService.GetProductByIdAndLanguage(productId,searchLanguage);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpdateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = await _productService.UpdateProduct(id,dto);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProduct(id);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("DeleteProductImage/{productId}/{mediaId}")]
        public async Task<IActionResult> DeleteProductImage(int productId, int mediaId)
        {
            try
            {
                var result = await _productService.DeleteProductImage(productId,mediaId);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetProductsByIngredientName/{ingredientName}")]
        public async Task<IActionResult> GetProductsByIngredientName(string ingredientName)
        {
            try
            {
                var result = await _productService.GetProductsByIngredientName(ingredientName);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetProductsByRegionName/{regionName}")]
        public async Task<IActionResult> GetProductsByregionName(string regionName)
        {
            try
            {
                var result = await _productService.GetProductsByRegionName(regionName);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetProductsByRegionNameAndSpecificLanguage/{regionName}/{shortCode}")]
        public async Task<IActionResult> GetProductsByregionName(string regionName,string shortCode)
        {
            try
            {
                var result = await _productService.GetProductsByRegionNameAndSpecificLanguage(regionName,shortCode);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
