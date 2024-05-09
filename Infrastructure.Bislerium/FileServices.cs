using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace Infrastructure.Bislerium
{
    public class FileServices
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IUrlHelper _urlHelper;

        public FileServices(IWebHostEnvironment environment, IUrlHelper urlHelper)
        {
            _environment = environment;
            _urlHelper = urlHelper;
        }

        public async Task<string?>? UploadImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null;
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return _urlHelper.Content($"~/uploads/{fileName}");
        }
    }
}
