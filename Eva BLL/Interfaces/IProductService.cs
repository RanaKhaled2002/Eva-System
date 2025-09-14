using Eva_DAL.DTOs.Ingredient;
using Eva_DAL.DTOs.Pagination;
using Eva_DAL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eva_DAL.DTOs.Product;

namespace Eva_BLL.Interfaces
{
    public interface IProductService
    {
        public Task<object> AddProduct(ProductCreateDTO dto);
        public Task<PaginatedResult<ProductDTO>> GetAllProducts(PaginationParams pagination);
        public Task<ProductDTO> GetProductById(int id);
        public Task<object> TranslateProduct(int productId);
        public Task<object> TransalteProductForSpecificLanguage(int productId, string languageCode);
        public Task<object> UpdateTransalteProductForSpecificLanguage(int productId, string languageCode, UpdateProductTranslationDto updatedTranslation);
        public Task<object> DeleteProductForSpecificLanguage(int productId, string LanguageCode);
        public Task<object> GetAllProductsBySpecificLanguage(string searchLanguage);
        public Task<object> GetProductByIdAndLanguage(int productId, string searchLanguage);
        public Task<object> UpdateProduct(int id, ProductUpdateDTO dto);
        public Task<object> DeleteProduct(int id);
        public Task<object> DeleteProductImage(int productId, int mediaId);
        public Task<List<ProductSearchDto>> GetProductsByIngredientName(string ingredientName);
        public Task<List<ProductSearchDto>> GetProductsByRegionName(string regionName);
        public Task<List<ProductSearchDto>> GetProductsByRegionNameAndSpecificLanguage(string regionName, string languageCode);
    }
}
