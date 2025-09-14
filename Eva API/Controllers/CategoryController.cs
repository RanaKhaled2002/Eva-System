using AutoMapper;
using Eva_BLL.Interfaces;
using Eva_DAL.DTOs.Category;
using Eva_DAL.Models;
using GTranslate.Translators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eva_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDTO dto)
        {
            if (dto == null) return BadRequest("Invalid data");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var category = _mapper.Map<Eva_DAL.Models.Category>(dto);

            await _unitOfWork.Repository<Category>().AddAsync(category);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Category added", categoryId = category.Id });
        }

        [HttpPost("TranslateCategory/{categoryId}")]
        public async Task<IActionResult> TranslateCategory(int categoryId)
        {
            var languages = await _unitOfWork.Repository<Language>().GetAllAsync();

            var category = await _unitOfWork.Repository<Category>()
                                .GetByIdAsync(categoryId, include: q => q.Include(c => c.Translations));

            if (category == null)
                return NotFound("Category not found");

            var originalName = category.TranslationKey;
            var originalDesc = category.Description;

            var existingLanguageIds = category.Translations.Select(t => t.LanguageId).ToList();

            if (languages.All(lang => existingLanguageIds.Contains(lang.Id)))
            {
                return Ok(new { message = "Category is already translated into all available languages." });
            }

            ITranslator translator = new GoogleTranslator();

            foreach (var lang in languages)
            {
                if (existingLanguageIds.Contains(lang.Id))
                    continue; // اللغة موجودة، نعديها

                var translatedNameResult = await translator.TranslateAsync(originalName, lang.LangShortCode);
                var translatedName = translatedNameResult.Translation;

                string translatedDesc = null;
                if (!string.IsNullOrEmpty(originalDesc))
                {
                    var translatedDescResult = await translator.TranslateAsync(originalDesc, lang.LangShortCode);
                    translatedDesc = translatedDescResult.Translation;
                }

                category.Translations.Add(new CategoryTranslation
                {
                    LanguageId = lang.Id,
                    Name = translatedName,
                    Description = translatedDesc,
                    CategoryId = category.Id
                });
            }

            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Translations updated successfully." });
        }

        [HttpPost("TranslateCategoryToLanguage/{categoryId}/{languageCode}")]
        public async Task<IActionResult> TranslateCategoryToLanguage(int categoryId, string languageCode)
        {
            if (categoryId <= 0) return BadRequest("Enter valid id");

            if (string.IsNullOrWhiteSpace(languageCode))
                return BadRequest("Language code is required.");

            var language = await _unitOfWork.Repository<Language>()
                .GetAsync(l => l.LangShortCode.ToLower() == languageCode.ToLower());

            if (language == null)
                return NotFound("Language not found.");

            var category = await _unitOfWork.Repository<Category>()
                .GetByIdAsync(categoryId, include: q => q.Include(c => c.Translations));

            if (category == null)
                return NotFound("Category not found.");

            var alreadyTranslated = category.Translations.Any(t => t.LanguageId == language.Id);
            if (alreadyTranslated)
                return Ok(new { message = $"Category is already translated to language '{languageCode}'." });

            ITranslator translator = new GoogleTranslator();

            var translatedNameResult = await translator.TranslateAsync(category.TranslationKey, language.LangShortCode);
            var translatedName = translatedNameResult.Translation;

            string? translatedDesc = null;
            if (!string.IsNullOrEmpty(category.Description))
            {
                var translatedDescResult = await translator.TranslateAsync(category.Description, language.LangShortCode);
                translatedDesc = translatedDescResult.Translation;
            }

            category.Translations.Add(new CategoryTranslation
            {
                LanguageId = language.Id,
                Name = translatedName,
                Description = translatedDesc,
                CategoryId = category.Id
            });

            await _unitOfWork.CompleteAsync();

            return Ok(new { message = $"Category translated to language '{languageCode}' successfully." });
        }

        [HttpGet("GetAllCategoriesBySpecificLanguage/{searchLanguage}")]
        public async Task<IActionResult> GetAllCategoriesBySpecificLanguage(string searchLanguage)
        {
            if (string.IsNullOrWhiteSpace(searchLanguage))
                return BadRequest("Please Language");

            var language = await _unitOfWork.Repository<Language>()
                .GetAsync(l => l.Lang.ToLower() == searchLanguage.ToLower());

            if (language == null)
                return NotFound("Language not found");

            var categories = await _unitOfWork.Repository<Category>()
                .GetAllAsync(include: q => q.Include(c => c.Translations));

            if (categories == null || !categories.Any())
                return NotFound("Categories not found");

            var result = categories
                .Where(c => c.Translations.Any(t => t.LanguageId == language.Id))
                .Select(c => new CategoryWithTranslationsDTO
                {
                    Id = c.Id,
                    TranslationKey = c.TranslationKey,
                    Description = c.Description,
                    Translations = c.Translations
                        .Where(t => t.LanguageId == language.Id)
                        .Select(t => new CategoryTranslationDTO
                        {
                            Name = t.Name,
                            Description = t.Description,
                            LanguageCode = searchLanguage.Substring(0,2).ToLower()
                        }).ToList()
                }).ToList();

            return Ok(result);
        }

        [HttpGet("GetCategoryByIdAndSpecificLanguage/{categoryId}/{searchLanguage}")]
        public async Task<IActionResult> GetCategoryByIdAndLanguage(int categoryId, string searchLanguage)
        {
            if (categoryId <= 0)
                return BadRequest("Invalid category ID");

            if (string.IsNullOrWhiteSpace(searchLanguage) || searchLanguage.Length < 2)
                return BadRequest("Please enter valid language");

            var language = await _unitOfWork.Repository<Language>()
                .GetAsync(l => l.Lang.ToLower() == searchLanguage.ToLower());

            if (language == null)
                return NotFound("Language not found");

            var category = await _unitOfWork.Repository<Category>()
                .GetByIdAsync(categoryId, include: q => q.Include(c => c.Translations));

            if (category == null)
                return NotFound("Category not found");

            if (!category.Translations.Any(t => t.LanguageId == language.Id))
                return NotFound("Translation for the specified language not found");

            var result = new CategoryWithTranslationsDTO
            {
                Id = category.Id,
                TranslationKey = category.TranslationKey,
                Description = category.Description,
                Translations = category.Translations
                    .Where(t => t.LanguageId == language.Id)
                    .Select(t => new CategoryTranslationDTO
                    {
                        Name = t.Name,
                        Description = t.Description,
                        LanguageCode = searchLanguage.Substring(0, 2).ToLower()
                    }).ToList()
            };

            return Ok(result);
        }

        [HttpGet("GetAllCategoriesWithTranslations")]
        public async Task<IActionResult> GetAllCategoriesWithTranslations()
        {
            var categories = await _unitOfWork.Repository<Category>()
                                              .GetAllAsync(include: q =>
                                              q.Include(c => c.Translations)
                                              .ThenInclude(t => t.Language));

            var result = _mapper.Map<List<CategoryWithTranslationsDTO>>(categories);

            return Ok(result);
        }

        [HttpGet("GetCategoryByIdWithTranslations/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("Enter valid id");

            var category = await _unitOfWork.Repository<Category>()
                .GetByIdAsync(id, include: q => 
                q.Include(c => c.Translations)
                .ThenInclude(c=>c.Language));

            if (category == null) return NotFound("No category found.");

            var dto = _mapper.Map<CategoryWithTranslationsDTO>(category);

            return Ok(dto);
        }
    }
}
