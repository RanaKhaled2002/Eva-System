using AutoMapper;
using Eva_BLL.Interfaces;
using Eva_DAL.DTOs;
using Eva_DAL.DTOs.Ingredient;
using Eva_DAL.DTOs.Pagination;
using Eva_DAL.DTOs.Region;
using Eva_DAL.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_BLL.Repositories
{
    public class RegionService : IRegionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITranslateService _translate;
        private readonly IImageService _imageService;

        public RegionService(IUnitOfWork unitOfWork, IMapper mapper, ITranslateService translate, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _translate = translate;
            _imageService = imageService;
        }

        public async Task AddRegion(RegionCreateDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Invalid Data");

            var isExists = await _unitOfWork.Repository<Region>()
                .GetAsync(i => i.TranslationKey == dto.TranslationKey);

            if (isExists != null)
                throw new InvalidOperationException($"Region with TranslationKey '{dto.TranslationKey}' already exists.");

            var region = _mapper.Map<Region>(dto);

            if (dto.MainImage != null)
            {
                var mainImageUrl = await _imageService.UploadImageAsync(dto.MainImage, "RegionMedia");
                region.MainImageUrl = mainImageUrl;
            }

            region.RegionCulturalImages = new List<RegionCulturalImage>();
            if (dto.RegionCulturalImages != null && dto.RegionCulturalImages.Any())
            {
                var uploadedImages = await _imageService.UploadMultipleImagesAsync<RegionCulturalImage>(dto.RegionCulturalImages, "RegionMedia");

                foreach (var img in uploadedImages)
                {
                    region.RegionCulturalImages.Add(new RegionCulturalImage
                    {
                        ImageUrl = img.ImageUrl
                    });
                }
            }

            region.RegionHistoricalImages = new List<RegionHistoricalImage>();
            if (dto.RegionHistoricalImages != null && dto.RegionHistoricalImages.Any())
            {
                var uploadedImages = await _imageService.UploadMultipleImagesAsync<RegionHistoricalImage>(dto.RegionHistoricalImages, "RegionMedia");

                foreach (var img in uploadedImages)
                {
                    region.RegionHistoricalImages.Add(new RegionHistoricalImage
                    {
                        ImageUrl = img.ImageUrl
                    });
                }
            }

            if (dto.IngredientIds != null && dto.IngredientIds.Any())
            {
                var ingredients = new List<Ingredient>();

                foreach (var id in dto.IngredientIds)
                {
                    var ingredient = await _unitOfWork.Repository<Ingredient>().GetByIdAsync(id);
                    if (ingredient == null)
                        throw new ArgumentException($"Ingredient with ID {id} not found.");

                    ingredient.Region = region;
                    ingredients.Add(ingredient);
                }

                region.Ingredients = ingredients;
            }

            await _unitOfWork.Repository<Region>().AddAsync(region);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<PaginatedResult<RegionDTO>> GetAllRegions(PaginationParams pagination)
        {
            var regions = await _unitOfWork.Repository<Region>().GetAllAsync(include: q => q
            .Include(r => r.RegionCulturalImages)
            .Include(r => r.RegionHistoricalImages)
            .Include(r => r.Ingredients)
            .Include(r => r.Translations).ThenInclude(r => r.Language));

            if (regions == null) throw new InvalidOperationException("Region not found");

            var totalItems = regions.Count();
            var pagedRegions = regions
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            var result = _mapper.Map<List<RegionDTO>>(pagedRegions);

            return new PaginatedResult<RegionDTO>
            {
                TotalItems = totalItems,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalItems / pagination.PageSize),
                Data = result
            };
        }

        public async Task<RegionDTO> GetRegionById(int id)
        {
            if(id<=0) throw new ArgumentException("Invalid Id");

            var region = await _unitOfWork.Repository<Region>().GetByIdAsync(id,include: q => q
            .Include(r => r.RegionCulturalImages)
            .Include(r => r.RegionHistoricalImages)
            .Include(r => r.Ingredients)
            .Include(r => r.Translations).ThenInclude(r => r.Language));

            if (region == null) throw new InvalidOperationException("Region not found");

            var result = _mapper.Map<RegionDTO>(region);

            return (result);
        }

        public async Task<object> TransalteRegionForSpecificLanguage(int regionId, string languageCode)
        {
            if (regionId <= 0 || string.IsNullOrWhiteSpace(languageCode)) throw new ArgumentException("Invalid Data");

            var region = await _unitOfWork.Repository<Region>().GetByIdAsync(regionId, include: q => q.Include(p => p.Translations));

            if (region == null) throw new InvalidOperationException("region not found");

            var language = await _unitOfWork.Repository<Eva_DAL.Models.Language>()
                .GetAsync(l => l.LangShortCode.ToLower() == languageCode.ToLower());

            if (language == null)
                throw new InvalidOperationException("Language not found.");

            var alreadyTranslated = region.Translations.Any(t => t.LanguageId == language.Id);
            if (alreadyTranslated)
                return new { message = $"Ingredient is already translated to language '{languageCode}'." };

            var translatedName = await _translate.TranslateTextAsync(region.TranslationKey, language.LangShortCode);
            var translatedCulturalContent = await _translate.TranslateTextAsync(region.CulturalContent, language.LangShortCode);
            var translatedHistoricalContent = await _translate.TranslateTextAsync(region.HistoricalContent, language.LangShortCode);

            region.Translations.Add(new RegionTranslation
            {
                LanguageId = language.Id,
                Name = translatedName,
                CulturalContent = translatedCulturalContent,
                HistoricalContent = translatedHistoricalContent,
            });

            await _unitOfWork.CompleteAsync();

            return new { message = $"Ingredient translated to language '{languageCode}' successfully." };
        }

        public async Task<object> TranslateRegion(int regionId)
        {
            if (regionId <= 0) throw new ArgumentException("Invalid id");

            var langugaes = await _unitOfWork.Repository<Eva_DAL.Models.Language>().GetAllAsync();

            if (langugaes == null) throw new InvalidOperationException("No languages found");

            var region = await _unitOfWork.Repository<Region>().GetByIdAsync(regionId, include: q => q.Include(p => p.Translations));

            if (region == null) throw new InvalidOperationException("No region found");

            var OriginalName = region.TranslationKey;
            var OriginalCulturalContent = region.CulturalContent;
            var OriginalHistoricalContent = region.HistoricalContent;

            var exisitingLanguageId = region.Translations?.Select(t => t.LanguageId).ToList() ?? new List<int>();

            if (langugaes.All(lang => exisitingLanguageId.Contains(lang.Id)))
            {
                return new { message = "Region is already translated into all available languages." };
            }

            foreach (var lang in langugaes)
            {
                if (exisitingLanguageId.Contains(lang.Id))
                    continue;

                var translatedName = await _translate.TranslateTextAsync(OriginalName, lang.LangShortCode);
                var translatedCulturalContent = await _translate.TranslateTextAsync(OriginalCulturalContent, lang.LangShortCode);
                var translatedHistoricalContent = await _translate.TranslateTextAsync(OriginalHistoricalContent, lang.LangShortCode);

                region.Translations.Add(new RegionTranslation
                {
                    LanguageId = lang.Id,
                    Name = translatedName,
                    CulturalContent = translatedCulturalContent,
                    HistoricalContent = translatedHistoricalContent,
                });
            }


            await _unitOfWork.CompleteAsync();

            return new { message = "Translations updated successfully." };
        }

        public async Task<object> UpdateTransalteRegionForSpecificLanguage(int regionId, string languageCode, UpdateRegionTranslationDto updatedTranslation)
        {
            if (regionId <= 0 || string.IsNullOrWhiteSpace(languageCode))
                throw new ArgumentException("Invalid Data");

            var region = await _unitOfWork.Repository<Region>()
                .GetByIdAsync(regionId, include: q => q.Include(p => p.Translations));

            if (region == null)
                throw new InvalidOperationException("Region not found");

            var language = await _unitOfWork.Repository<Eva_DAL.Models.Language>()
                .GetAsync(l => l.LangShortCode.ToLower() == languageCode.ToLower());

            if (language == null)
                throw new InvalidOperationException("Language not found.");

            var translationToUpdate = region.Translations
                 .FirstOrDefault(t => t.LanguageId == language.Id);

            if (translationToUpdate == null)
                throw new InvalidOperationException("Translation not found for the specified language.");

            if (!string.IsNullOrWhiteSpace(updatedTranslation.Name))
                translationToUpdate.Name = updatedTranslation.Name;

            if (!string.IsNullOrWhiteSpace(updatedTranslation.CulturalContent))
                translationToUpdate.CulturalContent = updatedTranslation.CulturalContent;

            if (!string.IsNullOrWhiteSpace(updatedTranslation.HistoricalContent))
                translationToUpdate.HistoricalContent = updatedTranslation.HistoricalContent;

            await _unitOfWork.CompleteAsync();

            return new { message = "Translation updated successfully." };
        }

        public async Task<object> DeleteRegionForSpecificLanguage(int regionId, string LanguageCode)
        {
            if (regionId <= 0 || string.IsNullOrWhiteSpace(LanguageCode)) throw new ArgumentException("Invalid Data");

            var region = await _unitOfWork.Repository<Region>().GetByIdAsync(regionId, include: q => q.Include(p => p.Translations));

            if (region == null) throw new InvalidOperationException("Ingredient not found");

            var language = await _unitOfWork.Repository<Eva_DAL.Models.Language>()
                .GetAsync(l => l.LangShortCode.ToLower() == LanguageCode.ToLower());

            if (language == null)
                throw new InvalidOperationException("Language not found.");

            var translationToDelete = region.Translations
                 .FirstOrDefault(t => t.LanguageId == language.Id);

            if (translationToDelete == null)
                throw new InvalidOperationException("Translation not found for the specified language.");

            region.Translations.Remove(translationToDelete);
            await _unitOfWork.CompleteAsync();

            return new { message = "Translation deleted successfully." };
        }

        public async Task<object> GetAllRegionsBySpecificLanguage(string searchLanguage)
        {
            if (string.IsNullOrWhiteSpace(searchLanguage))
                throw new ArgumentNullException("Please enter language");

            var language = await _unitOfWork.Repository<Eva_DAL.Models.Language>()
                .GetAsync(l => l.Lang.ToLower() == searchLanguage.ToLower());

            if (language == null)
                throw new InvalidOperationException("Language not found");

            var regions = await _unitOfWork.Repository<Region>()
                .GetAllAsync(include: q => q.Include(c => c.Translations));

            if (regions == null || !regions.Any())
                throw new InvalidOperationException("Region not found");

            var result = regions
                .Where(c => c.Translations.Any(t => t.LanguageId == language.Id))
                .Select(c => new RegionSearchDTO
                {
                    Id = c.Id,
                    Name = c.TranslationKey,
                    RegionTranslations = c.Translations.Where(t => t.LanguageId == language.Id)
                        .Select(t => new RegionTranslationDTO
                        {
                            Name = t.Name,
                            CulturalContent = t.CulturalContent,
                            HistoricalContent = t.HistoricalContent,
                            LanguageCode = searchLanguage.Substring(0, 2).ToLower()
                        }).ToList()
                }).ToList();

            return (result);
        }

        public async Task<object> GetRegionByIdAndLanguage(int regionId, string searchLanguage)
        {
            if (regionId <= 0)
                throw new ArgumentException("Invalid ID");

            if (string.IsNullOrWhiteSpace(searchLanguage) || searchLanguage.Length < 2)
                throw new ArgumentException("Please enter valid language");

            var language = await _unitOfWork.Repository<Eva_DAL.Models.Language>()
                .GetAsync(l => l.Lang.ToLower() == searchLanguage.ToLower());

            if (language == null)
                throw new InvalidOperationException("Language not found");

            var region = await _unitOfWork.Repository<Region>()
                .GetByIdAsync(regionId, include: q => q.Include(c => c.Translations));

            if (region == null)
                throw new InvalidOperationException("Region not found");

            if (!region.Translations.Any(t => t.LanguageId == language.Id))
                throw new InvalidOperationException("Translation for the specified language not found");

            var result = new RegionSearchDTO
            {
                Id = region.Id,
                Name = region.TranslationKey,
                RegionTranslations = region.Translations
                    .Where(t => t.LanguageId == language.Id)
                    .Select(t => new RegionTranslationDTO
                    {
                        Name = t.Name,
                        CulturalContent = t.CulturalContent,
                        HistoricalContent = t.HistoricalContent,
                        LanguageCode = searchLanguage.Substring(0, 2).ToLower()
                    }).ToList()
            };

            return result;
        }

        public async Task<object> UpdateRegion(int id,RegionUpdateDTO dto)
        {
            if (dto == null || id <= 0)
                throw new ArgumentException("Invalid data");

            var region = await _unitOfWork.Repository<Region>()
                .GetByIdAsync(id, include: q => q
                    .Include(r => r.RegionCulturalImages)
                    .Include(r => r.RegionHistoricalImages)
                    .Include(r => r.Ingredients)
                    .Include(r => r.Translations).ThenInclude(r => r.Language));

            if (region == null)
                throw new InvalidOperationException("Region not found");

            var isExists = await _unitOfWork.Repository<Region>()
                .GetAsync(i => i.TranslationKey == dto.TranslationKey);

            if (isExists != null)
                throw new InvalidOperationException($"Region with TranslationKey '{dto.TranslationKey}' already exists.");

            region.TranslationKey = dto.TranslationKey ?? region.TranslationKey;
            region.Name = dto.Name ?? region.Name;
            region.CulturalContent = dto.CulturalContent ?? region.CulturalContent;
            region.HistoricalContent = dto.HistoricalContent ?? region.HistoricalContent;

            if (dto.MainImage != null && dto.MainImage.Length > 0)
            {
                var mainImageUrl = await _imageService.UploadImageAsync(dto.MainImage, "RegionMedia");
                region.MainImageUrl = mainImageUrl;
            }

            if (dto.RegionCulturalImages != null && dto.RegionCulturalImages.Any())
            {
                var uploadedImages = await _imageService.UploadMultipleImagesAsync<RegionCulturalImage>(dto.RegionCulturalImages, "RegionMedia");

                foreach (var img in uploadedImages)
                {
                    region.RegionCulturalImages.Add(new RegionCulturalImage
                    {
                        ImageUrl = img.ImageUrl
                    });
                }
            }

            if (dto.RegionHistoricalImages != null && dto.RegionHistoricalImages.Any())
            {
                var uploadedImages = await _imageService.UploadMultipleImagesAsync<RegionHistoricalImage>(dto.RegionHistoricalImages, "RegionMedia");

                foreach (var img in uploadedImages)
                {
                    region.RegionHistoricalImages.Add(new RegionHistoricalImage
                    {
                        ImageUrl = img.ImageUrl
                    });
                }
            }

            if (dto.IngredientIds != null && dto.IngredientIds.Any())
            {
                var existingIngredientIds = region.Ingredients.Select(i => i.Id).ToList();
                var distinctIds = dto.IngredientIds.Distinct();

                foreach (var ingredientId in distinctIds)
                {
                    if (!existingIngredientIds.Contains(ingredientId))
                    {
                        var ingredient = await _unitOfWork.Repository<Ingredient>().GetByIdAsync(ingredientId);
                        if (ingredient == null)
                            throw new ArgumentException($"Ingredient with ID {ingredientId} not found.");

                        ingredient.Region = region;
                        region.Ingredients.Add(ingredient);
                    }
                }
            }

            var languages = await _unitOfWork.Repository<Eva_DAL.Models.Language>().GetAllAsync();

            foreach (var lang in languages)
            {
                var existingTranslation = region.Translations.FirstOrDefault(t => t.LanguageId == lang.Id);

                var translatedName = dto.TranslationKey != null
                    ? await _translate.TranslateTextAsync(dto.TranslationKey, lang.LangShortCode)
                    : existingTranslation?.Name;

                var translatedCulturalContent = dto.CulturalContent != null
                    ? await _translate.TranslateTextAsync(dto.CulturalContent, lang.LangShortCode)
                    : existingTranslation?.CulturalContent;

                var translatedHistoricalContent = dto.HistoricalContent != null
                    ? await _translate.TranslateTextAsync(dto.HistoricalContent, lang.LangShortCode)
                    : existingTranslation?.HistoricalContent;

                if (existingTranslation != null)
                {
                    existingTranslation.Name = translatedName;
                    existingTranslation.CulturalContent = translatedCulturalContent;
                    existingTranslation.HistoricalContent = translatedHistoricalContent;
                }
            }

            await _unitOfWork.CompleteAsync();

            return new { message = "Ingredient and translations updated successfully." };
        }

        public async Task<object> DeleteRegion(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid Id");

            var region = await _unitOfWork.Repository<Region>().GetByIdAsync(id);

            if (region == null) throw new InvalidOperationException("Region not found");

            await _unitOfWork.Repository<Region>().Delete(region);
            await _unitOfWork.CompleteAsync();

            return "Region Deleted Successfully";
        }

        public async Task<object> DeleteMainImage(int regionId)
        {
            if (regionId <= 0)
                throw new ArgumentException("Invalid Region ID");

            var region = await _unitOfWork.Repository<Region>()
                .GetByIdAsync(regionId);

            if (region == null)
                throw new InvalidOperationException("Region not found.");

            if (string.IsNullOrEmpty(region.MainImageUrl))
                throw new ArgumentException("Main image does not exist.");

            var isDeleted = _imageService.DeleteImage(region.MainImageUrl);
            if (!isDeleted)
                throw new ArgumentException("Failed to delete the main image from the server.");

            region.MainImageUrl = null;
            await _unitOfWork.CompleteAsync();

            return new { message = "Main image deleted successfully." };
        }

        public async Task<object> DeleteRegionCultureImage(int regionId, int mediaId)
        {
            if (regionId <= 0 || mediaId <= 0)
                throw new ArgumentException("Invalid Data");

            var region = await _unitOfWork.Repository<Region>()
                .GetByIdAsync(regionId, include: q => q.Include(p => p.RegionCulturalImages));

            if (region == null)
                throw new InvalidOperationException("Region not found.");

            var media = region.RegionCulturalImages.FirstOrDefault(m => m.Id == mediaId);
            if (media == null)
                throw new InvalidOperationException("Image not found.");

            var isDeleted = _imageService.DeleteImage(media.ImageUrl);
            if (!isDeleted)
                throw new ArgumentException("Failed to delete image");

            region.RegionCulturalImages.Remove(media);
            await _unitOfWork.CompleteAsync();

            return new { message = "Image deleted successfully." };
        }

        public async Task<object> DeleteRegionHistroicalImage(int regionId, int mediaId)
        {
            if (regionId <= 0 || mediaId <= 0)
                throw new ArgumentException("Invalid Data");

            var region = await _unitOfWork.Repository<Region>()
                .GetByIdAsync(regionId, include: q => q.Include(p => p.RegionHistoricalImages));

            if (region == null)
                throw new InvalidOperationException("Region not found.");

            var media = region.RegionHistoricalImages.FirstOrDefault(m => m.Id == mediaId);
            if (media == null)
                throw new InvalidOperationException("Image not found.");

            var isDeleted = _imageService.DeleteImage(media.ImageUrl);
            if (!isDeleted)
                throw new ArgumentException("Failed to delete image");

            region.RegionHistoricalImages.Remove(media);
            await _unitOfWork.CompleteAsync();

            return new { message = "Image deleted successfully." };
        }
    }
}
