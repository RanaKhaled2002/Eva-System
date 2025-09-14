using DocumentFormat.OpenXml.Office2010.Excel;
using Eva_BLL.Interfaces;
using Eva_DAL.DTOs;
using Eva_DAL.DTOs.Region;
using Microsoft.AspNetCore.Mvc;

namespace Eva_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionController : ControllerBase
    {
        private readonly IRegionService _regionService;

        public RegionController(IRegionService regionService)
        {
            _regionService = regionService;
        }

        [HttpPost("AddRegion")]
        public async Task<IActionResult> AddRegion([FromForm] RegionCreateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                await _regionService.AddRegion(dto);
                return Ok(new { message = $"Region added successfully" });
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

        [HttpGet("GetAllRegions")]
        public async Task<IActionResult> GetAllRegions([FromQuery]PaginationParams pagination)
        {
            try
            {
                var regions = await _regionService.GetAllRegions(pagination);
                return Ok(regions);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetRegionById/{id}")]
        public async Task<IActionResult> GetAllRegions(int id)
        {
            try
            {
                var region = await _regionService.GetRegionById(id);
                return Ok(region);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("TranslateRegion/{regionId}")]
        public async Task<IActionResult> TranslateRegion(int regionId)
        {
            try
            {
                var result = await _regionService.TranslateRegion(regionId);
                return Ok(result);
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

        [HttpPost("TranslateRegion/{regionId}/{languageCode}")]
        public async Task<IActionResult> TransaltsRegionForSpecificLanguage(int regionId,string languageCode)
        {
            try
            {
                var result = await _regionService.TransalteRegionForSpecificLanguage(regionId,languageCode);
                return Ok(result);
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

        [HttpPut("UpdateSpecificTranslationFromSpecificRegion/{regionId}/{languageCode}")]
        public async Task<IActionResult> UpdateSpecificTranslationFromSpecificRegion(int regionId, string languageCode, [FromBody] UpdateRegionTranslationDto updatedTranslation)
        {
            try
            {
                var result = await _regionService.UpdateTransalteRegionForSpecificLanguage(regionId, languageCode, updatedTranslation);
                return Ok(result);
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

        [HttpDelete("DeleteSpecificTranslationFromSpecificRegion/{regionId}/{LanguageCode}")]
        public async Task<IActionResult> DeleteSpecificTranslationFromSpecificRegion(int regionId, string LanguageCode)
        {
            try
            {
                var result = await _regionService.DeleteRegionForSpecificLanguage(regionId, LanguageCode);
                return Ok(result);
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

        [HttpGet("GetAllRegionsBySpecificLanguage/{searchLanguage}")]
        public async Task<IActionResult> GetAllRegionsBySpecificLanguage(string searchLanguage)
        {
            try
            {
                var result = await _regionService.GetAllRegionsBySpecificLanguage(searchLanguage);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetRegionByIdAndSpecificLanguage/{regionId}/{searchLanguage}")]
        public async Task<IActionResult> GetRegionByIdAndLanguage(int regionId, string searchLanguage)
        {
            try
            {
                var result = await _regionService.GetRegionByIdAndLanguage(regionId,searchLanguage);
                return Ok(result);
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

        [HttpPut("UpdateRegion/{id}")]
        public async Task<IActionResult> UpdateRegion(int id, [FromForm] RegionUpdateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = await _regionService.UpdateRegion(id,dto);
                return Ok(result);
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

        [HttpDelete("DeleteRegion/{id}")]
        public async Task<IActionResult> DeleteRegion(int id)
        {
            try
            {
                var result = await _regionService.DeleteRegion(id);
                return Ok(result);
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

        [HttpDelete("DeleteMainImage/{regionId}")]
        public async Task<IActionResult> DeleteMainImage(int regionId)
        {
            try
            {
                var result = await _regionService.DeleteMainImage(regionId);
                return Ok(result);
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

        [HttpDelete("DeleteRegionCulturalImages/{regionId}/{mediaId}")]
        public async Task<IActionResult> DeleteRegionCulturalImages(int regionId, int mediaId)
        {
            try
            {
                var result = await _regionService.DeleteRegionCultureImage(regionId,mediaId);
                return Ok(result);
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

        [HttpDelete("DeleteRegionHistoricalImages/{regionId}/{mediaId}")]
        public async Task<IActionResult> DeleteRegionHistoricalImages(int regionId, int mediaId)
        {
            try
            {
                var result = await _regionService.DeleteRegionHistroicalImage(regionId, mediaId);
                return Ok(result);
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
