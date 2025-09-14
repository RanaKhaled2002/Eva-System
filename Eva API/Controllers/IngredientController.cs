using Eva_BLL.Interfaces;
using Eva_DAL.DTOs;
using Eva_DAL.DTOs.Ingredient;
using Microsoft.AspNetCore.Mvc;

namespace Eva_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;

        public IngredientController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        [HttpPost("AddIngredient")]
        public async Task<IActionResult> AddIngredient([FromForm] IngredientCreateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = await _ingredientService.AddIngredient(dto);
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

        [HttpGet("GetAllIngredient")]
        public async Task<IActionResult> GetAllIngredient([FromQuery]PaginationParams pagination)
        {
            try
            {
                var result = await _ingredientService.GetAllIngredients(pagination);
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

        [HttpGet("GetIngredientById/{id}")]
        public async Task<IActionResult> GetIngredientById(int id)
        {
            try
            {
                var result = await _ingredientService.GetIngredientById(id);
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

        [HttpPost("TranslateIngredient/{IngredientId}")]
        public async Task<IActionResult> TranslateIngredient(int IngredientId)
        {
            try
            {
                var result = await _ingredientService.TranslateIngredient(IngredientId);
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

        [HttpPost("TranslateIngredient/{IngredientId}/{LanguageCode}")]
        public async Task<IActionResult> TranslateIngredientForSpecificLanguage(int IngredientId, string LanguageCode)
        {
            try
            {
                var result = await _ingredientService.TransalteIngredientForSpecificLanguage(IngredientId,LanguageCode);
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

        [HttpPut("UpdateSpecificTranslationFromSpecificIngredient/{IngredientId}/{LanguageCode}")]
        public async Task<IActionResult> UpdateIngredientForSpecificLanguage(int IngredientId, string LanguageCode, [FromBody] UpdateIngredientTranslationDto updatedTranslation)
        {
            try
            {
                var result = await _ingredientService.UpdateTransalteIngredientForSpecificLanguage(IngredientId,LanguageCode, updatedTranslation);
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

        [HttpDelete("DeleteSpecificTranslationFromSpecificIngredient/{IngredientId}/{LanguageCode}")]
        public async Task<IActionResult> DeleteIngredientForSpecificLanguage(int IngredientId, string LanguageCode)
        {
            try
            {
                var result = await _ingredientService.DeleteIngredientForSpecificLanguage(IngredientId,LanguageCode);
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

        [HttpGet("GetAllIngredientsBySpecificLanguage/{searchLanguage}")]
        public async Task<IActionResult> GetAllIngredientsBySpecificLanguage(string searchLanguage)
        {
            try
            {
                var result = await _ingredientService.GetAllIngredientsBySpecificLanguage(searchLanguage);
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

        [HttpGet("GetIngredientByIdAndSpecificLanguage/{ingredientId}/{searchLanguage}")]
        public async Task<IActionResult> GetIngredientByIdAndLanguage(int ingredientId, string searchLanguage)
        {
            try
            {
                var result = await _ingredientService.GetIngredientByIdAndLanguage(ingredientId,searchLanguage);
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

        [HttpPut("UpdateIngredient/{id}")]
        public async Task<IActionResult> UpdateIngredient(int id, [FromForm] IngredientUpdateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = await _ingredientService.UpdateIngredient(id,dto);
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

        [HttpDelete("DeleteIngredient/{id}")]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            try
            {
                var result = await _ingredientService.DeleteIngredient(id);
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

        [HttpDelete("DeleteIngredientImages/{ingredientId}/{mediaId}")]
        public async Task<IActionResult> DeleteIngredientImage(int ingredientId, int mediaId)
        {
            try
            {
                var result = await _ingredientService.DeleteDeleteIngredientImage(ingredientId,mediaId);
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

        [HttpDelete("DeleteMainImage/{ingredientId}")]
        public async Task<IActionResult> DeleteMainImage(int ingredientId)
        {
            try
            {
                var result = await _ingredientService.DeleteMainImage(ingredientId);
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
