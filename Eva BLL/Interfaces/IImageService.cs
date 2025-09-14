using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eva_BLL.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file, string folderName);

        Task<List<T>> UploadMultipleImagesAsync<T>(IEnumerable<IFormFile> files, string folderName)
            where T : class, new();

        bool DeleteImage(string relativePath);
    }
}
