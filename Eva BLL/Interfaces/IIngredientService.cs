using Eva_DAL.DTOs.Pagination;
using Eva_DAL.DTOs.Region;
using Eva_DAL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eva_DAL.DTOs.Ingredient;

namespace Eva_BLL.Interfaces
{
    public interface IIngredientService
    {
        public Task<object> AddIngredient(IngredientCreateDTO dto);
        public Task<PaginatedResult<IngredientDTO>> GetAllIngredients(PaginationParams pagination);
        public Task<IngredientDTO> GetIngredientById(int id);
        public Task<object> TranslateIngredient(int ingredientId);
        public Task<object> TransalteIngredientForSpecificLanguage(int ingredientId, string languageCode);
        public Task<object> UpdateTransalteIngredientForSpecificLanguage(int ingredientId, string languageCode, UpdateIngredientTranslationDto updatedTranslation);
        public Task<object> DeleteIngredientForSpecificLanguage(int IngredientId, string LanguageCode);
        public Task<object> GetAllIngredientsBySpecificLanguage(string searchLanguage);
        public Task<object> GetIngredientByIdAndLanguage(int ingredientId, string searchLanguage);
        public Task<object> UpdateIngredient(int id, IngredientUpdateDTO dto);
        public Task<object> DeleteIngredient(int id);
        public Task<object> DeleteMainImage(int ingredientId);
        public Task<object> DeleteDeleteIngredientImage(int ingredientId, int mediaId);
    }
}
