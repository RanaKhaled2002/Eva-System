using AutoMapper;
using Eva_BLL.Interfaces;
using Eva_DAL.DTOs.Language;
using Eva_DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eva_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LanguageController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LanguageController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost("AddLanguage")]
        public async Task<IActionResult> AddLanguage([FromBody]LanguageDTO languageDTO)
        {
            if (languageDTO == null) return BadRequest("Language Data is null");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _mapper.Map<Language>(languageDTO);

            await _unitOfWork.Repository<Language>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return Ok("Language Added Successfully");
        }

        [HttpGet("GetAllLanguages")]
        public async Task<IActionResult> GetAllLanguage()
        {
            var languages = await _unitOfWork.Repository<Language>().GetAllAsync();

            if (languages == null) return NotFound("No Data found");

            var result = _mapper.Map<List<ShowLanguageDTO>>(languages);

            return Ok(result);
        }

        [HttpGet("GetLanguageById/{id}")]
        public async Task<IActionResult> GetLangauageById(int id)
        {
            if (id <= 0) return BadRequest("Enter valid id");

            var lang = await _unitOfWork.Repository<Language>().GetByIdAsync(id);

            if (lang == null) return NotFound("Language not found");

            var result = _mapper.Map<ShowLanguageDTO>(lang);

            return Ok(result);
        }

        [HttpPut("UpdateLanguage/{id}")]
        public async Task<IActionResult> UpdateLanguage(int id, [FromBody] UpdateLanguageDTO updateLanguageDTO)
        {
            if (updateLanguageDTO == null || id <= 0)
                return BadRequest("Invalid Data");

            var existingLang = await _unitOfWork.Repository<Language>().GetByIdAsync(id);

            if (existingLang == null)
                return NotFound("Language not found");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!string.IsNullOrWhiteSpace(updateLanguageDTO.Lang))
                existingLang.Lang = updateLanguageDTO.Lang;

            if (!string.IsNullOrWhiteSpace(updateLanguageDTO.LangShortCode))
                existingLang.LangShortCode = updateLanguageDTO.LangShortCode;

            if (!string.IsNullOrWhiteSpace(updateLanguageDTO.Direction))
                existingLang.Direction = updateLanguageDTO.Direction;

            await _unitOfWork.Repository<Language>().Update(existingLang);
            await _unitOfWork.CompleteAsync();

            return Ok("Language Updated Successfully");
        }

        [HttpDelete("DeleteLanguage/{id}")]
        public async Task<IActionResult> DeleteLanguage(int id)
        {
            if (id <= 0) return BadRequest("Invalid Id");

            var lang = await _unitOfWork.Repository<Language>().GetByIdAsync(id);

            if (lang == null) return NotFound("Language not found");

            await _unitOfWork.Repository<Language>().Delete(lang);
            await _unitOfWork.CompleteAsync();

            return Ok("Language deleted successfully");
        }
    }
}
