using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace API.Services
{
    public interface IFileService
    {
        string ReadFormFile(IFormFile formFile);

        Task<string> ReadFormFileAsync(IFormFile formFile);
    }
}