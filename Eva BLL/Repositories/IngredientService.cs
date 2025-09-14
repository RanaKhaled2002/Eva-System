using AutoMapper;
using Eva_BLL.Interfaces;
using Eva_DAL.DTOs;
using Eva_DAL.DTOs.Ingredient;
using Eva_DAL.DTOs.Pagination;
using Eva_DAL.DTOs.Region;
using Eva_DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_BLL.Repositories
{
    public class IngredientService : IIngredientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITranslateService _translate;
        private readonly IImageService _imageService;

        public IngredientService(IUnitOfWork unitOfWork, IMapper mapper, ITranslateService translate, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _translate = translate;
            _imageService = imageService;
        }

        public async Task<object> AddIngredient(IngredientCreateDTO dto)
        {
            if (dto == null) throw new ArgumentException("Invalid Data");


            var isExists = await _unitOfWork.Repository<Ingredient>()
                  .GetAsync(i => i.TranslationKey == dto.TranslationKey);

            if (isExists != null)
                throw new ArgumentException($"Ingredient with TranslationKey '{dto.TranslationKey}' already exists.");

            var ingredient = _mapper.Map<Ingredient>(dto);

            ingredient.IngredientImages = new List<IngredientImage>();

            if (dto.IngredientImages != null && dto.IngredientImages.Any())
            {
                var uploadedImages = await _imageService.UploadMultipleImagesAsync<IngredientImage>(dto.IngredientImages, "IngredientMedia");

                foreach (var img in uploadedImages)
                {
                    ingredient.IngredientImages.Add(img);
                }
            }

            if (dto.MainImage != null)
            {
                var mainImageUrl = await _imageService.UploadImageAsync(dto.MainImage, "IngredientMedia");
                ingredient.MainIngredientImageUrl = mainImageUrl;
            }

            if (dto.ProductIds != null && dto.ProductIds.Any())
            {
                ingredient.ProductIngredients = new List<ProductIngredient>();
                foreach (var id in dto.ProductIds)
                {
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
                    if (product == null)
                        throw new ArgumentException($"Product with ID {id} not found.");

                    ingredient.ProductIngredients.Add(new ProductIngredient
                    {
                        ProductId = id,
                        Ingredient = ingredient
                    });
                }
            }

            await _unitOfWork.Repository<Ingredient>().AddAsync(ingredient);
            await _unitOfWork.CompleteAsync();

            return "Ingredient added";
        }

        public async Task<PaginatedResult<IngredientDTO>> GetAllIngredients(PaginationParams pagination)
        {
            var ingredients = await _unitOfWork.Repository<Ingredient>().GetAllAsync(include: q => q
                                .Include(I => I.IngredientImages)
                                .Include(I => I.Translations).ThenInclude(I => I.Language)
                                .Include(I => I.ProductIngredients).ThenInclude(I => I.Product));

            if (ingredients == null) throw new InvalidOperationException("Ingredients Not Found");

            var totalItems = ingredients.Count();
            var pagedRegions = ingredients
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            var result = _mapper.Map<List<IngredientDTO>>(pagedRegions);

            return new PaginatedResult<IngredientDTO>
            {
                TotalItems = totalItems,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalItems / pagination.PageSize),
                Data = result
            };
        }

        public async Task<IngredientDTO> GetIngredientById(int id)
        {
            if (id <= 0) throw new ArgumentException("Enter Valid Id");

            var ingredient = await _unitOfWork.Repository<Ingredient>().GetByIdAsync(id, include: q => q
                                              .Include(I => I.IngredientImages)
                                              .Include(I => I.ProductIngredients).ThenInclude(I => I.Product)
                                              .Include(I => I.Translations).ThenInclude(I => I.Language));

            if (ingredient == null) throw new InvalidOperationException("Ingredient not found");

            var result = _mapper.Map<IngredientDTO>(ingredient);

            return (result);
        }

        public async Task<object> TranslateIngredient(int ingredientId)
        {
            if (ingredientId <= 0) throw new ArgumentException("Invalid id");

            var langugaes = await _unitOfWork.Repository<Eva_DAL.Models.Language>().GetAllAsync();

            if (langugaes == null) throw new InvalidOperationException("No languages found");

            var ingredient = await _unitOfWork.Repository<Ingredient>().GetByIdAsync(ingredientId, include: q => q.Include(p => p.Translations));

            if (ingredient == null) throw new InvalidOperationException("No ingredient found");

            var OriginalName = ingredient.TranslationKey;
            var OriginalIngredientStory = ingredient.Story;
            var OriginalBenefits = ingredient.Benefits;

            var exisitingLanguageId = ingredient.Translations?.Select(t => t.LanguageId).ToList() ?? new List<int>();

            if (langugaes.All(lang => exisitingLanguageId.Contains(lang.Id)))
            {
                return  "Ingredient is already translated into all available languages." ;
            }

            foreach (var lang in langugaes)
            {
                if (exisitingLanguageId.Contains(lang.Id))
                    continue;

                var translatedName = await _translate.TranslateTextAsync(OriginalName, lang.LangShortCode);
                var translatedStory = await _translate.TranslateTextAsync(OriginalIngredientStory, lang.LangShortCode);
                var translatedBenefits = await _translate.TranslateTextAsync(OriginalBenefits, lang.LangShortCode);

                ingredient.Translations.Add(new IngredientTranslation
                {
                    LanguageId = lang.Id,
                    Name = translatedName,
                    Story = translatedStory,
                    Benfits = translatedBenefits,
                });
            }


            await _unitOfWork.CompleteAsync();

            return "Translations updated successfully.";
        }

        public async Task<object> TransalteIngredientForSpecificLanguage(int ingredientId, string languageCode)
        {
            if (ingredientId <= 0 || string.IsNullOrWhiteSpace(languageCode)) throw new ArgumentException("Invalid Data");

            var ingredient = await _unitOfWork.Repository<Ingredient>().GetByIdAsync(ingredientId, include: q => q.Include(p => p.Translations));

            if (ingredient == null) throw new InvalidOperationException("Ingredient not found");

            var language = await _unitOfWork.Repository<Eva_DAL.Models.Language>()
                .GetAsync(l => l.LangShortCode.ToLower() == languageCode.ToLower());

            if (language == null)
                throw new InvalidOperationException("Language not found.");

            var alreadyTranslated = ingredient.Translations.Any(t => t.LanguageId == language.Id);
            if (alreadyTranslated)
                return $"Ingredient is already translated to language '{languageCode}'.";

            var translatedName = await _translate.TranslateTextAsync(ingredient.TranslationKey, language.LangShortCode);
            var translatedStory = await _translate.TranslateTextAsync(ingredient.Story, language.LangShortCode);
            var translatedBenefits = await _translate.TranslateTextAsync(ingredient.Benefits, language.LangShortCode);

            ingredient.Translations.Add(new IngredientTranslation
            {
                LanguageId = language.Id,
                Name = translatedName,
                Story = translatedStory,
                Benfits = translatedBenefits
            });

            await _unitOfWork.CompleteAsync();

            return $"Ingredient translated to language '{languageCode}' successfully.";
        }

        public async Task<object> UpdateTransalteIngredientForSpecificLanguage(int ingredientId, string languageCode, UpdateIngredientTranslationDto updatedTranslation)
        {
            if (ingredientId <= 0 || string.IsNullOrWhiteSpace(languageCode))
                throw new ArgumentException("Invalid Data");

            var ingredient = await _unitOfWork.Repository<Ingredient>()
                .GetByIdAsync(ingredientId, include: q => q.Include(p => p.Translations));

            if (ingredient == null)
                throw new InvalidOperationException("Ingredient not found");

            var language = await _unitOfWork.Repository<Eva_DAL.Models.Language>()
                .GetAsync(l => l.LangShortCode.ToLower() == languageCode.ToLower());

            if (language == null)
                throw new InvalidOperationException("Language not found.");

            var translationToUpdate = ingredient.Translations
                 .FirstOrDefault(t => t.LanguageId == language.Id);

            if (translationToUpdate == null)
                throw new InvalidOperationException("Translation not found for the specified language.");

            if (!string.IsNullOrWhiteSpace(updatedTranslation.Name))
                translationToUpdate.Name = updatedTranslation.Name;

            if (!string.IsNullOrWhiteSpace(updatedTranslation.IngredientStory))
                translationToUpdate.Story = updatedTranslation.IngredientStory;

            if (!string.IsNullOrWhiteSpace(updatedTranslation.Benefits))
                translationToUpdate.Benfits = updatedTranslation.Benefits;

            await _unitOfWork.CompleteAsync();

            return "Translation updated successfully.";
        }

        public async Task<object> DeleteIngredientForSpecificLanguage(int IngredientId, string LanguageCode)
        {
            if (IngredientId <= 0 || string.IsNullOrWhiteSpace(LanguageCode)) throw new ArgumentException("Invalid Data");

            var ingredient = await _unitOfWork.Repository<Ingredient>().GetByIdAsync(IngredientId, include: q => q.Include(p => p.Translations));

            if (ingredient == null) throw new InvalidOperationException("Ingredient not found");

            var language = await _unitOfWork.Repository<Eva_DAL.Models.Language>()
                .GetAsync(l => l.LangShortCode.ToLower() == LanguageCode.ToLower());

            if (language == null)
                throw new InvalidOperationException("Language not found.");

            var translationToDelete = ingredient.Translations
                 .FirstOrDefault(t => t.LanguageId == language.Id);

            if (translationToDelete == null)
                throw new InvalidOperationException("Translation not found for the specified language.");

            ingredient.Translations.Remove(translationToDelete);
            await _unitOfWork.CompleteAsync();

            return "Translation deleted successfully.";
        }

        public async Task<object> GetAllIngredientsBySpecificLanguage(string searchLanguage)
        {
            if (string.IsNullOrWhiteSpace(searchLanguage))
                throw new ArgumentException("Please enter language");

            var language = await _unitOfWork.Repository<Eva_DAL.Models.Language>()
                .GetAsync(l => l.Lang.ToLower() == searchLanguage.ToLower());

            if (language == null)
                throw new InvalidOperationException("Language not found");

            var ingredients = await _unitOfWork.Repository<Ingredient>()
                .GetAllAsync(include: q => q.Include(c => c.Translations));

            if (ingredients == null || !ingredients.Any())
                throw new InvalidOperationException("Ingredients not found");

            var result = ingredients
                .Where(c => c.Translations.Any(t => t.LanguageId == language.Id))
                .Select(c => new IngredientSearchDto
                {
                    Id = c.Id,
                    Name = c.TranslationKey,
                    IngredientTranslations = c.Translations.Where(t => t.LanguageId == language.Id)
                        .Select(t => new IngredientTranlationDTO
                        {
                            Name = t.Name,
                            IngredientStory = t.Story,
                            Benefits = t.Benfits,
                            LanguageCode = searchLanguage.Substring(0, 2).ToLower()
                        }).ToList()
                }).ToList();

            return result;
        }

        public async Task<object> GetIngredientByIdAndLanguage(int ingredientId, string searchLanguage)
        {
            if (ingredientId <= 0)
                throw new ArgumentException("Invalid ID");

            if (string.IsNullOrWhiteSpace(searchLanguage) || searchLanguage.Length < 2)
                throw new ArgumentException("Please enter valid language");

            var language = await _unitOfWork.Repository<Eva_DAL.Models.Language>()
                .GetAsync(l => l.Lang.ToLower() == searchLanguage.ToLower());

            if (language == null)
                throw new InvalidOperationException("Language not found");

            var ingredient = await _unitOfWork.Repository<Ingredient>()
                .GetByIdAsync(ingredientId, include: q => q.Include(c => c.Translations));

            if (ingredient == null)
                throw new InvalidOperationException("Ingredient not found");

            if (!ingredient.Translations.Any(t => t.LanguageId == language.Id))
                throw new InvalidOperationException("Translation for the specified language not found");

            var result = new IngredientSearchDto
            {
                Name = ingredient.TranslationKey,
                IngredientTranslations = ingredient.Translations
                    .Where(t => t.LanguageId == language.Id)
                    .Select(t => new IngredientTranlationDTO
                    {
                        Name = t.Name,
                        IngredientStory = t.Story,
                        Benefits = t.Benfits,
                        LanguageCode = searchLanguage.Substring(0, 2).ToLower()
                    }).ToList()
            };

            return result;
        }

        public async Task<object> UpdateIngredient(int id, IngredientUpdateDTO dto)
        {
            if (dto == null || id <= 0)
                throw new ArgumentException("Invalid data");

            var ingredient = await _unitOfWork.Repository<Ingredient>()
                .GetByIdAsync(id, include: q => q
                    .Include(i => i.IngredientImages)
                    .Include(i => i.Translations).ThenInclude(t => t.Language)
                    .Include(i => i.ProductIngredients).ThenInclude(pi => pi.Product));

            if (ingredient == null)
                throw new InvalidOperationException("Ingredient not found");

            var isExists = await _unitOfWork.Repository<Ingredient>()
                  .GetAsync(i => i.TranslationKey == dto.TranslationKey);

            if (isExists != null)
                throw new ArgumentException($"Ingredient with TranslationKey '{dto.TranslationKey}' already exists.");

            ingredient.TranslationKey = dto.TranslationKey ?? ingredient.TranslationKey;
            ingredient.Name = dto.Name ?? ingredient.Name;
            ingredient.Story = dto.IngredientStory ?? ingredient.Story;
            ingredient.Benefits = dto.Benefits ?? ingredient.Benefits;

            if (dto.MainImage != null && dto.MainImage.Length > 0)
            {
                var mainImageUrl = await _imageService.UploadImageAsync(dto.MainImage, "IngredientMedia");
                ingredient.MainIngredientImageUrl = mainImageUrl;
            }

            if (dto.IngredientImages != null && dto.IngredientImages.Any())
            {
                var uploadedImages = await _imageService.UploadMultipleImagesAsync<IngredientImage>(dto.IngredientImages, "IngredientMedia");
                foreach (var img in uploadedImages)
                {
                    ingredient.IngredientImages.Add(img);
                }
            }

            if (dto.ProductIds != null && dto.ProductIds.Any())
            {
                var existingProductIds = ingredient.ProductIngredients.Select(pi => pi.ProductId).ToList();
                var distinctProductIds = dto.ProductIds.Distinct();

                foreach (var productId in distinctProductIds)
                {
                    if (!existingProductIds.Contains(productId))
                    {
                        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
                        if (product == null)
                           throw new ArgumentException($"Product with ID {productId} not found.");

                        ingredient.ProductIngredients.Add(new ProductIngredient
                        {
                            ProductId = productId,
                            IngredientId = ingredient.Id
                        });
                    }
                }
            }

            var languages = await _unitOfWork.Repository<Eva_DAL.Models.Language>().GetAllAsync();

            foreach (var lang in languages)
            {
                var existingTranslation = ingredient.Translations.FirstOrDefault(t => t.LanguageId == lang.Id);

                // لو القيم الأساسية مش null، نترجمها، وإلا نخلي الترجمة زي ما هي
                var translatedName = dto.TranslationKey != null
                    ? await _translate.TranslateTextAsync(dto.TranslationKey, lang.LangShortCode)
                    : existingTranslation?.Name;

                var translatedStory = dto.IngredientStory != null
                    ? await _translate.TranslateTextAsync(dto.IngredientStory, lang.LangShortCode)
                    : existingTranslation?.Story;

                var translatedBenefits = dto.Benefits != null
                    ? await _translate.TranslateTextAsync(dto.Benefits, lang.LangShortCode)
                    : existingTranslation?.Benfits;

                if (existingTranslation != null)
                {
                    existingTranslation.Name = translatedName;
                    existingTranslation.Story = translatedStory;
                    existingTranslation.Benfits = translatedBenefits;
                }
                else if (translatedName != null) // لو مفيش ترجمة قبل كده، نضيف واحدة جديدة فقط لو في اسم مترجم
                {
                    ingredient.Translations.Add(new IngredientTranslation
                    {
                        LanguageId = lang.Id,
                        Name = translatedName,
                        Story = translatedStory,
                        Benfits = translatedBenefits
                    });
                }
            }

            await _unitOfWork.CompleteAsync();

            return "Ingredient and translations updated successfully.";
        }

        public async Task<object> DeleteIngredient(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid Id");

            var ingredient = await _unitOfWork.Repository<Ingredient>().GetByIdAsync(id);

            if (ingredient == null) throw new InvalidCastException("ingredient not found");

            await _unitOfWork.Repository<Ingredient>().Delete(ingredient);
            await _unitOfWork.CompleteAsync();

            return "Ingredient Deleted Successfully";
        }

        public async Task<object> DeleteMainImage(int ingredientId)
        {
            if (ingredientId <= 0)
                throw new ArgumentException("Invalid Ingredient ID");

            var ingredient = await _unitOfWork.Repository<Ingredient>()
                .GetByIdAsync(ingredientId);

            if (ingredient == null)
                throw new InvalidOperationException("Ingredient not found.");

            if (string.IsNullOrEmpty(ingredient.MainIngredientImageUrl))
                throw new ArgumentException("Main image does not exist.");

            var isDeleted = _imageService.DeleteImage(ingredient.MainIngredientImageUrl);
            if (!isDeleted)
                throw new ArgumentException("Failed to delete the main image from the server.");

            ingredient.MainIngredientImageUrl = null;
            await _unitOfWork.CompleteAsync();

            return "Main image deleted successfully.";
        }

        public async Task<object> DeleteDeleteIngredientImage(int ingredientId, int mediaId)
        {
            if (ingredientId <= 0 || mediaId <= 0)
                throw new ArgumentException("Invalid Data");

            var ingredient = await _unitOfWork.Repository<Ingredient>()
                .GetByIdAsync(ingredientId, include: q => q.Include(p => p.IngredientImages));

            if (ingredient == null)
                throw new InvalidOperationException("Ingredient not found.");

            var media = ingredient.IngredientImages.FirstOrDefault(m => m.Id == mediaId);
            if (media == null)
                throw new InvalidOperationException("Image not found.");

            var isDeleted = _imageService.DeleteImage(media.ImageUrl);
            if (!isDeleted)
                throw new ArgumentException("Failed to delete image");

            ingredient.IngredientImages.Remove(media);
            await _unitOfWork.CompleteAsync();

            return "Image deleted successfully." ;
        }
    }
}
