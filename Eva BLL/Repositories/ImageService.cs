using Eva_BLL.Interfaces;
using Eva_DAL.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_BLL.Repositories
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _env;

        public ImageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploads = Path.Combine(_env.WebRootPath, "uploads", folderName);
            Directory.CreateDirectory(uploads);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var path = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{folderName}/{fileName}";
        }

        public async Task<List<T>> UploadMultipleImagesAsync<T>(IEnumerable<IFormFile> files, string folderName) where T : class, new()
        {
            var result = new List<T>();

            if (files == null) return result;

            foreach (var file in files)
            {
                var imageUrl = await UploadImageAsync(file, folderName);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    var imageInstance = new T();

                    var property = typeof(T).GetProperty("ImageUrl");
                    if (property != null && property.CanWrite)
                    {
                        property.SetValue(imageInstance, imageUrl);
                    }

                    result.Add(imageInstance);
                }
            }

            return result;
        }


        public bool DeleteImage(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return false;

            var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }

            return false;
        }
    }
}
