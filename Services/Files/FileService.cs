using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ShopEProduction.Services.Files
{
    public class FileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderPath, string fileNamePrefix)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file provided for upload.");
            }

            if (string.IsNullOrEmpty(_environment.WebRootPath))
            {
                throw new InvalidOperationException("WebRootPath is not initialized.");
            }

            // Construct full folder path
            var fullFolderPath = Path.Combine(_environment.WebRootPath, folderPath);
            Directory.CreateDirectory(fullFolderPath);

            // Get file extension and construct full file name
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{fileNamePrefix}{extension}";
            var filePath = Path.Combine(fullFolderPath, fileName);

            // Delete existing file if it exists
            if (File.Exists(filePath))
            {
                Console.WriteLine($"Deleting existing file: {filePath}");
                File.Delete(filePath);
            }

            // Save the new file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return the relative URL path
            return $"/{folderPath}/{fileName}";
        }

        public void DeleteFile(string filePath)
        {
            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                Console.WriteLine($"Deleting file: {fullPath}");
                File.Delete(fullPath);
            }
        }
    }
}