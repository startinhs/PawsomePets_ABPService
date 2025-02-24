using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace nRetailApp.Services.FileService;

public interface IFileService
{
    IFormFile ConvertBase64ToIFormFile(string base64);
}