using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PawsomePets.AbpBlobContainers;
using PawsomePets.MediaStorages;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace PawsomePets.Controllers
{
    [RemoteService]
    [Area("app")]
    [ControllerName("MediaStorages")]
    public class MediaStorageController : AbpController
    {
        protected IMediaStoragesAppService _mediaStoragesAppService;
        public MediaStorageController(IMediaStoragesAppService mediaStoragesAppService)
        {
            _mediaStoragesAppService = mediaStoragesAppService;
        }

        [Route("media/{fileName}")]
        [HttpGet]
        public async Task<object> GetFileUrl(string fileName)
        {
            return await _mediaStoragesAppService.GetImageByFileName(fileName, isDownload: false);
        }

        [Route("download-file/{fileName}")]
        [HttpGet]
        public async Task<object> DownloadFile(string fileName)
        {
            return await _mediaStoragesAppService.GetImageByFileName(fileName, isDownload: true);
        }

        [Route("upload-file")]
        [HttpPost]
        public async Task<object> UploadFile([FromBody] FileUpload fileUpload)
        {
            return await _mediaStoragesAppService.UploadFile(fileUpload);
        }

        [Route("delete-file")]
        [HttpDelete]
        public async Task<object> DeleteFileByAbp(string fileName)
        {
            return await _mediaStoragesAppService.DeleteFile(fileName);
        }
    }
}
