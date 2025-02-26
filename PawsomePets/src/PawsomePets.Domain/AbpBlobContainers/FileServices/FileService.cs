using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using static System.Net.Mime.MediaTypeNames;

namespace PawsomePets.Services.FileService;

public class FileService : IFileService
{
    public IFormFile ConvertBase64ToIFormFile(string base64)
    {
        var _base64 = base64.Trim();

        if (_base64.Length % 4 == 0 && Regex.IsMatch(_base64, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None))
        {
            var bytes = Convert.FromBase64String(base64);
            var stream = new MemoryStream(bytes);
            IFormFile file = new FormFile(stream, 0, stream.Length, null, Guid.NewGuid() + ".jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = Image.Jpeg
            };
            return file;
        }

        throw new InvalidOperationException("Base64 not match");
    }
}