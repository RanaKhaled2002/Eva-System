using AutoMapper;
using Eva_BLL.Interfaces;
using Eva_DAL.DTOs;
using Eva_DAL.DTOs.Ingredient;
using Eva_DAL.DTOs.Pagination;
using Eva_DAL.DTOs.Product;
using Eva_DAL.Models;
using GTranslate.Translators;
using Microsoft.EntityFrameworkCore;

namespace Eva_BLL.Repositories
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITranslateService _translate;
        private readonly IImageService _imageService;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ITranslateService translate, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _translate = translate;
            _imageService = imageService;
        }

        public async Task<object> AddProduct(ProductCreateDTO dto)
        {
            if (dto == null) throw new ArgumentException("Invalid Data");

            var isExists = await _unitOfWork.Repository<Product>()
                  .GetAsync(i => i.TranslationKey == dto.TranslationKey);

            if (isExists != null)
                throw new ArgumentException($"Ingredient with TranslationKey '{dto.TranslationKey}' already exists.");

            var product = _mapper.Map<Product>(dto);

            product.ProductMedias = new List<ProductMedia>();

            if (dto.ProductMedias != null && dto.ProductMedias.Any())
            {
                var uploadedImages = await _imageService.UploadMultipleImagesAsync<ProductMedia>(dto.ProductMedias, "ProductMedia");

                foreach (var img in uploadedImages)
                {
                    product.ProductMedias.Add(img);
                }
            }

            if (dto.IngredientIds != null && dto.IngredientIds.Any())
            {
                product.ProductIngredients = new List<ProductIngredient>();
                foreach (var id in dto.IngredientIds)
                {
                    var ingredient = await _unitOfWork.Repository<Ingredient>().GetByIdAsync(id);
                    if (ingredient == null)
                        throw new ArgumentException($"Ingredient with ID {id} not found.");

                    product.ProductIngredients.Add(new ProductIngredient
                    {
                        IngredientId = id,
                        Product = product
                    });
                }
            }

            // التحقق من التصنيف
            var category = await _unitOfWork.Repository<Eva_DAL.Models.Category>().GetByIdAsync(dto.categoryId);
            if (category == null) throw new ArgumentException("No Category Found");
            product.categoryId = dto.categoryId;

            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.CompleteAsync();

            return "Product added";
        }

        public async Task<PaginatedResult<ProductDTO>> GetAllProducts(PaginationParams pagination)
        {
            var products = await _unitOfWork.Repository<Product>().GetAllAsync(include: q => q
                .Include(p => p.Category)
                .Include(p => p.ProductIngredients).ThenInclude(pi => pi.Ingredient)
                .Include(p => p.Translations).ThenInclude(t => t.Language)
                .Include(p => p.ProductMedias));

            if (products == null) throw new InvalidOperationException("No data found");

            var totalItems = products.Count();
            var pagedRegions = products
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            var result = _mapper.Map<List<ProductDTO>>(pagedRegions);


            return new PaginatedResult<ProductDTO>
            {
                TotalItems = totalItems,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalItems / pagination.PageSize),
                Data = result
            };
        }

        public async Task<ProductDTO> GetProductById(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid id");

            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id, include: q => q
                                           .Include(p => p.Category)
                                           .Include(p => p.ProductIngredients).ThenInclude(p => p.Ingredient)
                                           .Include(p => p.Translations).ThenInclude(p => p.Language)
                                           .Include(p => p.ProductMedias));

            if (product == null) throw new InvalidOperationException("product not found");

            var result = _mapper.Map<ProductDTO>(product);

            return result;
        }

        public async Task<object> TranslateProduct(int productId)
        {
            if (productId <= 0) throw new ArgumentException("Invalid id");

            var langugaes = await _unitOfWork.Repository<Eva_DAL.Models.Language>().GetAllAsync();

            if (langugaes == null) throw new InvalidOperationException("No languages found");

            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId, include: q => q.Include(p => p.Translations));

            if (product == null) throw new InvalidOperationException("No product found");

            var OriginalName = product.TranslationKey;
            var OriginalMediaDescription = product.MediaDescription;
            var OriginalInstruction = product.Instruction;

            var exisitingLanguageId = product.Translations?.Select(t => t.LanguageId).ToList() ?? new List<int>();

            if (langugaes.All(lang => exisitingLanguageId.Contains(lang.Id)))
            {
                return "Product is already translated into all available languages.";
            }

            ITranslator translator = new GoogleTranslator();

            foreach (var lang in langugaes)
            {
                if (exisitingLanguageId.Contains(lang.Id))
                    continue; // اللغة موجودة، نعديها

                var translatedNameResult = await translator.TranslateAsync(OriginalName, lang.LangShortCode);
                var translatedName = translatedNameResult.Translation;

                string translatedDesc = null;
                if (!string.IsNullOrEmpty(OriginalMediaDescription))
                {
                    var translatedDescResult = await translator.TranslateAsync(OriginalMediaDescription, lang.LangShortCode);
                    translatedDesc = translatedDescResult.Translation;
                }

                string translatedIns = null;
                if (!string.IsNullOrEmpty(OriginalInstruction))
                {
                    var translatedInsResult = await translator.TranslateAsync(OriginalInstruction, lang.LangShortCode);
                    translatedIns = translatedInsResult.Translation;
                }

                product.Translations.Add(new ProductTranslation
                {
                    LanguageId = lang.Id,
                    Name = translatedName,
                    MediaDescription = translatedDesc,
                    Instruction = translatedIns,
                });

            }


            await _unitOfWork.CompleteAsync();

            return "Translations updated successfully." ;
        }

        public async Task<object> TransalteProductForSpecificLanguage(int productId, string languageCode)
        {
            if (productId <= 0 || string.IsNullOrWhiteSpace(languageCode)) throw new ArgumentException("Invalid Data");

            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId, include: q => q.Include(p => p.Translations));

            if (product == null) throw new InvalidOperationException("Product not found");

            var language = await _unitOfWork.Repository<Eva_DAL.Models.Language>()
                .GetAsync(l => l.LangShortCode.ToLower() == languageCode.ToLower());

            if (language == null)
                throw new InvalidOperationException("Language not found.");

            var alreadyTranslated = product.Translations.Any(t => t.LanguageId == language.Id);
            if (alreadyTranslated)
                return $"Product is already translated to language '{languageCode}'." ;

            ITranslator translator = new GoogleTranslator();

            var translatedNameResult = await translator.TranslateAsync(product.TranslationKey, language.LangShortCode);
            var translatedName = translatedNameResult.Translation;

            string? translatedDesc = null;
            if (!string.IsNullOrEmpty(product.MediaDescription))
            {
                var translatedDescResult = await translator.TranslateAsync(product.MediaDescription, language.LangShortCode);
                translatedDesc = translatedDescResult.Translation;
            }

            string? translatedIns = null;
            if (!string.IsNullOrEmpty(product.Instruction))
            {
                var translatedDescResult = await translator.TranslateAsync(product.Instruction, language.LangShortCode);
                translatedIns = translatedDescResult.Translation;
            }

            product.Translations.Add(new ProductTranslation
            {
                LanguageId = language.Id,
                Name = translatedName,
                MediaDescription = translatedDesc,
                Instruction = translatedIns
            });

            await _unitOfWork.CompleteAsync();

            return $"Product translated to language '{languageCode}' successfully.";
        }

        public async Task<object> UpdateTransalteProductForSpecificLanguage(int productId, string languageCode, UpdateProductTranslationDto updatedTranslation)
        {
            if (productId <= 0 || string.IsNullOrWhiteSpace(languageCode))
                throw new ArgumentException("Invalid Data");

            var ingredient = await _unitOfWork.Repository<Product>()
                .GetByIdAsync(productId, include: q => q.Include(p => p.Translations));

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

            translationToUpdate.Name = !string.IsNullOrWhiteSpace(updatedTranslation.Name)
                ? updatedTranslation.Name
                : translationToUpdate.Name;

            translationToUpdate.MediaDescription = !string.IsNullOrWhiteSpace(updatedTranslation.MediaDescription)
                ? updatedTranslation.MediaDescription
                : translationToUpdate.MediaDescription;

            translationToUpdate.Instruction = !string.IsNullOrWhiteSpace(updatedTranslation.Instruction)
                ? updatedTranslation.Instruction
                : translationToUpdate.Instruction;


            await _unitOfWork.CompleteAsync();

            return "Translation updated successfully." ;
        }

        public async Task<object> DeleteProductForSpecificLanguage(int productId, string LanguageCode)
        {
            if (productId <= 0 || string.IsNullOrWhiteSpace(LanguageCode)) throw new ArgumentException("Invalid Data");

            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId, include: q => q.Include(p => p.Translations));

            if (product == null) throw new InvalidOperationException("Product not found");

            var language = await _unitOfWork.Repository<Eva_DAL.Models.Language>()
                .GetAsync(l => l.LangShortCode.ToLower() == LanguageCode.ToLower());

            if (language == null)
                throw new InvalidOperationException("Language not found.");

            var translationToDelete = product.Translations
                 .FirstOrDefault(t => t.LanguageId == language.Id);

            if (translationToDelete == null)
                throw new InvalidOperationException("Translation not found for the specified language.");

            product.Translations.Remove(translationToDelete);
            await _unitOfWork.CompleteAsync();

            return "Translation deleted successfully." ;
        }

        public async Task<object> GetAllProductsBySpecificLanguage(string searchLanguage)
        {
            if (string.IsNullOrWhiteSpace(searchLanguage))
                throw new ArgumentException("Please enter language");

            var language = await _unitOfWork.Repository<Eva_DAL.Models.Language>()
                .GetAsync(l => l.Lang.ToLower() == searchLanguage.ToLower());

            if (language == null)
                throw new InvalidOperationException("Language not found");

            var products = await _unitOfWork.Repository<Product>()
                .GetAllAsync(include: q => q.Include(c => c.Translations));

            if (products == null || !products.Any())
                throw new InvalidOperationException("Products not found");

            var result = products
                .Where(c => c.Translations.Any(t => t.LanguageId == language.Id))
                .Select(c => new ProductSearchDto
                {
                    Id = c.Id,
                    Name = c.TranslationKey,
                    ProductTranslations = c.Translations.Where(t => t.LanguageId == language.Id)
                        .Select(t => new ProductTranlationDto
                        {
                            Name = t.Name,
                            MediaDescription = t.MediaDescription,
                            Instruction = t.Instruction,
                            LanguageCode = searchLanguage.Substring(0, 2).ToLower()
                        }).ToList()
                }).ToList();

            return result;
        }

        public async Task<object> GetProductByIdAndLanguage(int productId, string searchLanguage)
        {
            if (productId <= 0)
                throw new ArgumentException("Invalid ID");

            if (string.IsNullOrWhiteSpace(searchLanguage) || searchLanguage.Length < 2)
                throw new ArgumentException("Please enter valid language");

            var language = await _unitOfWork.Repository<Eva_DAL.Models.Language>()
                .GetAsync(l => l.Lang.ToLower() == searchLanguage.ToLower());

            if (language == null)
                throw new InvalidOperationException("Language not found");

            var product = await _unitOfWork.Repository<Product>()
                .GetByIdAsync(productId, include: q => q.Include(c => c.Translations));

            if (product == null)
                throw new InvalidOperationException("Product not found");

            if (!product.Translations.Any(t => t.LanguageId == language.Id))
                throw new InvalidOperationException("Translation for the specified language not found");

            var result = new ProductSearchDto
            {
                Id = product.Id,
                Name = product.TranslationKey,
                ProductTranslations = product.Translations
                    .Where(t => t.LanguageId == language.Id)
                    .Select(t => new ProductTranlationDto
                    {
                        Name = t.Name,
                        MediaDescription = t.MediaDescription,
                        Instruction = t.Instruction,
                        LanguageCode = searchLanguage.Substring(0, 2).ToLower()
                    }).ToList()
            };

            return result;
        }

        public async Task<object> UpdateProduct(int id, ProductUpdateDTO dto)
        {
            if (dto == null || id <= 0) throw new ArgumentException("Invalid data");

            var product = await _unitOfWork.Repository<Product>()
                .GetByIdAsync(id, include: q => q
                    .Include(p => p.Translations)
                    .Include(p => p.ProductIngredients)
                    .Include(p => p.ProductMedias));

            if (product == null) throw new InvalidOperationException("Product not found");

            var isExists = await _unitOfWork.Repository<Product>()
                  .GetAsync(i => i.TranslationKey == dto.TranslationKey && i.Id != id);

            if (isExists != null)
                throw new ArgumentException($"Product with TranslationKey '{dto.TranslationKey}' already exists.");

            product.TranslationKey = dto.TranslationKey ?? product.TranslationKey;
            product.Name = dto.Name ?? product.Name;
            product.Size = dto.Size ?? product.Size;
            product.Quantity = dto.Quantity.HasValue ? dto.Quantity.Value : product.Quantity;
            product.Instruction = dto.Instruction ?? product.Instruction;
            product.MediaDescription = dto.MediaDescription ?? product.MediaDescription;

            if (dto.ProductMedias != null && dto.ProductMedias.Any())
            {
                var uploadedImages = await _imageService.UploadMultipleImagesAsync<ProductMedia>(dto.ProductMedias, "ProductMedia");
                foreach (var img in uploadedImages)
                {
                    product.ProductMedias.Add(img);
                }
            }

            if (dto.categoryId.HasValue)
            {
                var category = await _unitOfWork.Repository<Eva_DAL.Models.Category>().GetByIdAsync(dto.categoryId.Value);
                if (category == null)
                    throw new InvalidOperationException("Category not found");

                product.categoryId = dto.categoryId;
            }

            if (dto.IngredientIds != null && dto.IngredientIds.Any())
            {
                var existingIngredientIds = product.ProductIngredients.Select(pi => pi.IngredientId).ToList();
                var distinctIngredientIds = dto.IngredientIds.Distinct();

                foreach (var ingredientId in distinctIngredientIds)
                {
                    if (!existingIngredientIds.Contains(ingredientId))
                    {
                        var ingredient = await _unitOfWork.Repository<Ingredient>().GetByIdAsync(ingredientId);
                        if (ingredient == null)
                            throw new ArgumentException($"Ingredient with ID {ingredientId} not found.");

                        product.ProductIngredients.Add(new ProductIngredient
                        {
                            IngredientId = ingredientId,
                            Product = product
                        });
                    }
                }
            }

            var languages = await _unitOfWork.Repository<Eva_DAL.Models.Language>().GetAllAsync();
            ITranslator translator = new GoogleTranslator();

            foreach (var lang in languages)
            {
                var existingTranslation = product.Translations.FirstOrDefault(t => t.LanguageId == lang.Id);

                string translatedDesc;
                if (!string.IsNullOrEmpty(dto.MediaDescription))
                {
                    translatedDesc = (await translator.TranslateAsync(dto.MediaDescription, lang.LangShortCode)).Translation;
                }
                else
                {
                    translatedDesc = existingTranslation?.MediaDescription;
                }

                string translatedIns;
                if (!string.IsNullOrEmpty(dto.Instruction))
                {
                    translatedIns = (await translator.TranslateAsync(dto.Instruction, lang.LangShortCode)).Translation;
                }
                else
                {
                    translatedIns = existingTranslation?.Instruction;
                }

                string translatedName;
                if (!string.IsNullOrEmpty(dto.TranslationKey))
                {
                    translatedName = (await translator.TranslateAsync(dto.TranslationKey, lang.LangShortCode)).Translation;
                }
                else
                {
                    translatedName = existingTranslation?.Name;
                }

                if (existingTranslation != null)
                {
                    existingTranslation.Name = translatedName;
                    existingTranslation.MediaDescription = translatedDesc;
                    existingTranslation.Instruction = translatedIns;
                }
            }

            await _unitOfWork.CompleteAsync();

            return "Product and translations updated successfully.";
        }

        public async Task<object> DeleteProduct(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid id");

            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

            if (product == null) throw new InvalidOperationException("Product Not Found");

            await _unitOfWork.Repository<Product>().Delete(product);
            await _unitOfWork.CompleteAsync();

            return "deleted successfully";
        }

        public async Task<object> DeleteProductImage(int productId, int mediaId)
        {
            if (productId <= 0 || mediaId <= 0)
                throw new ArgumentException("Invalid Data");

            var product = await _unitOfWork.Repository<Product>()
                .GetByIdAsync(productId, include: q => q.Include(p => p.ProductMedias));

            if (product == null)
                throw new InvalidOperationException("Product not found.");

            var media = product.ProductMedias.FirstOrDefault(m => m.Id == mediaId);
            if (media == null)
                throw new InvalidOperationException("Image not found.");

            var isDeleted = _imageService.DeleteImage(media.ImageUrl);
            if (!isDeleted)
                throw new ArgumentException("Failed to delete image");

            product.ProductMedias.Remove(media);
            await _unitOfWork.CompleteAsync();

            return "Image deleted successfully.";
        }

        public async Task<List<ProductSearchDto>> GetProductsByIngredientName(string ingredientName)
        {
            if (string.IsNullOrWhiteSpace(ingredientName))
                throw new ArgumentException("Ingredient name is required");

            var ingredients = await _unitOfWork.Repository<Ingredient>()
                .GetAllAsync(filter: i => i.Name.ToLower().Contains(ingredientName.ToLower()),include: q => q
                .Include(i => i.ProductIngredients)
                .ThenInclude(pi => pi.Product)
                .ThenInclude(p => p.Translations)
                );

            var products = ingredients
                .SelectMany(i => i.ProductIngredients)
                .Select(pi => pi.Product)
                .Distinct()
                .ToList();

            var result = products.Select(p => new ProductSearchDto
            {
                Id = p.Id,
                Name = p.TranslationKey,
                ProductTranslations = p.Translations?.Select(t => new ProductTranlationDto
                {
                    Name = t.Name,
                    MediaDescription = t.MediaDescription,
                    Instruction = t.Instruction,
                    LanguageCode = t.Language?.Lang.ToLower()
                }).ToList()
            }).ToList();

            return result;
        }

        public async Task<List<ProductSearchDto>> GetProductsByRegionName(string regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName))
                throw new ArgumentException("Region name is required");

            var ingredients = await _unitOfWork.Repository<Ingredient>()
                .GetAllAsync(filter: i => i.Region.Name.ToLower().Contains(regionName.ToLower()),include: q => q
                .Include(i => i.Region)
                .Include(i => i.ProductIngredients)
                .ThenInclude(pi => pi.Product)
                .ThenInclude(p => p.Translations)
                );

            var products = ingredients
                .SelectMany(i => i.ProductIngredients)
                .Select(pi => pi.Product)
                .Distinct()
                .ToList();

            var result = products.Select(p => new ProductSearchDto
            {
                Id = p.Id,
                Name = p.TranslationKey,
                ProductTranslations = p.Translations?.Select(t => new ProductTranlationDto
                {
                    Name = t.Name,
                    MediaDescription = t.MediaDescription,
                    Instruction = t.Instruction,
                    LanguageCode = t.Language?.Lang.ToLower()
                }).ToList()
            }).ToList();

            return result;
        }

        public async Task<List<ProductSearchDto>> GetProductsByRegionNameAndSpecificLanguage(string regionName, string languageCode)
        {
            if (string.IsNullOrWhiteSpace(regionName))
                throw new ArgumentException("Region name is required");

            if (string.IsNullOrWhiteSpace(languageCode) || languageCode.Length < 2)
                throw new ArgumentException("Valid language code is required (e.g. 'ar', 'en')");

            var language = await _unitOfWork.Repository<Language>().GetAsync(l => l.LangShortCode.ToLower() == languageCode.ToLower());

            if (language == null) throw new InvalidOperationException("No language found");

            var ingredients = await _unitOfWork.Repository<Ingredient>()
                .GetAllAsync(filter: i => i.Region.Name.ToLower().Contains(regionName.ToLower()),include: q => q
                .Include(i => i.Region)
                .Include(i => i.ProductIngredients)
                .ThenInclude(pi => pi.Product)
                .ThenInclude(p => p.Translations)
                .ThenInclude(t => t.Language)
                );

            var products = ingredients
                .SelectMany(i => i.ProductIngredients)
                .Select(pi => pi.Product)
                .Distinct()
                .ToList();

            var result = products.Select(p =>
            {
                var translation = p.Translations?.FirstOrDefault(t =>
                    t.Language != null &&
                    t.Language.Lang.ToLower().StartsWith(languageCode.ToLower()));

                return new ProductSearchDto
                {
                    Id = p.Id,
                    Name = p.TranslationKey,
                    ProductTranslations = translation != null
                        ? new List<ProductTranlationDto>
                        {
                            new ProductTranlationDto
                            {
                                Name = translation.Name,
                                MediaDescription = translation.MediaDescription,
                                Instruction = translation.Instruction,
                                LanguageCode = translation.Language.Lang.ToLower()
                            }
                        }
                        : new List<ProductTranlationDto>() 
                };
            }).ToList();

            return result;
        }
    }
}
