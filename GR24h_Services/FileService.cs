using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using GR24h_Repos;

namespace GR24h_Services
{
    public class FileService
    {
        private readonly string _webRootPath;

        public FileService(string webRootPath)
        {
            _webRootPath = webRootPath;
        }

        public string SaveImage(Stream fileStream, string originalFileName, string folderName)
        {
            string uploadsFolder = Path.Combine(_webRootPath, folderName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + originalFileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
           
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                fileStream.CopyTo(stream);
            }

            return uniqueFileName;
        }

        public void DeleteImage(string fileName, string folderName)
        {
            if (string.IsNullOrEmpty(fileName) || fileName == "default-recipe.jpg")
                return;

            string filePath = Path.Combine(_webRootPath, folderName, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
