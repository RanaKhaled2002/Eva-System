using Eva_DAL.DTOs;
using Eva_DAL.DTOs.Pagination;
using Eva_DAL.DTOs.Region;


namespace Eva_BLL.Interfaces
{
    public interface IRegionService
    {
        public Task AddRegion(RegionCreateDTO dto);
        public Task<PaginatedResult<RegionDTO>> GetAllRegions(PaginationParams pagination);
        public Task<RegionDTO> GetRegionById(int id);
        public Task<object> TranslateRegion(int regionId);
        public Task<object> TransalteRegionForSpecificLanguage(int regionId, string languageCode);
        public Task<object> UpdateTransalteRegionForSpecificLanguage(int regionId, string languageCode,  UpdateRegionTranslationDto updatedTranslation);
        public Task<object> DeleteRegionForSpecificLanguage(int IngredientId, string LanguageCode);
        public Task<object> GetAllRegionsBySpecificLanguage(string searchLanguage);
        public Task<object> GetRegionByIdAndLanguage(int regionId, string searchLanguage);
        public Task<object> UpdateRegion(int id,RegionUpdateDTO dto);
        public Task<object> DeleteRegion(int id);
        public Task<object> DeleteMainImage(int regionId);
        public Task<object> DeleteRegionCultureImage(int regionId, int mediaId);
        public Task<object> DeleteRegionHistroicalImage(int regionId, int mediaId);
    }
}
