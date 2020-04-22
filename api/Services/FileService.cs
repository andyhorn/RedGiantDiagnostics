using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace API.Services
{
    public class FileService : IFileService
    {
        public string ReadFormFile(IFormFile formFile)
        {
            if (formFile == null)
            {
                throw new ArgumentNullException("formFile", "FormFile cannot be null");
            }

            if (formFile.Length == 0)
            {
                throw new ArgumentException("File length cannot be zero", "formFile");
            }
            
            string data = string.Empty;

            using (var stream = formFile.OpenReadStream())
            {
                var length = stream.Length;

                byte[] bytes = new byte[length];

                stream.Read(bytes, 0, (int)length);

                data = Encoding.UTF8.GetString(bytes, 0, (int)length);
            }

            return data;
        }

        public async Task<string> ReadFormFileAsync(IFormFile formFile)
        {
            string data = null;
            await Task.Run(() => {
                data = ReadFormFile(formFile);
            });
            return data;
        }
    }
}