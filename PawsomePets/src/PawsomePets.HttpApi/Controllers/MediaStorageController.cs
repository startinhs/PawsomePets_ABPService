using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PawsomePets.AbpBlobContainers;
using PawsomePets.MediaStorages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.BlobStoring;

namespace PawsomePets.Controllers
{
    [RemoteService]
    [Area("app")]
    [ControllerName("MediaStorages")]
    public class MediaStorageController : AbpController
    {
        protected IMediaStoragesAppService _mediaStoragesAppService;
        private readonly IBlobContainer<DatabaseContainer> _databaseContainer;
        private readonly IBlobContainer<AwsContainer> _awsContainer;
        private readonly IBlobContainer<AzureContainer> _azureContainer;
        public MediaStorageController(IMediaStoragesAppService mediaStoragesAppService, IBlobContainer<DatabaseContainer> databaseContainer,
        IBlobContainer<AwsContainer> awsContainer, IBlobContainer<AzureContainer> azureContainer)
        {
            _mediaStoragesAppService = mediaStoragesAppService;
            _databaseContainer = databaseContainer;
            _awsContainer = awsContainer;
            _azureContainer = azureContainer;
        }

        [Route("image/{name}")]
        [HttpGet]
        public async Task<IActionResult> GetBlob(string name)
        {
            var blob = await _databaseContainer.GetAllBytesAsync(name);
            return new FileContentResult(blob, "image/jpeg");
        }

        [Route("image-aws/{name}")]
        [HttpGet]
        public async Task<IActionResult> GetBlobAws(string name)
        {
            var blob = await _awsContainer.GetAllBytesAsync(name);
            return new FileContentResult(blob, "image/jpeg");
        }

        [Route("image-azure/{name}")]
        [HttpGet]
        public async Task<IActionResult> GetBlobAzure(string name)
        {
            var blob = await _azureContainer.GetAllBytesAsync(name);
            return new FileContentResult(blob, "image/jpeg");
        }

        [Route("upload-image")]
        [HttpPost]
        public async Task<object> UploadImage([FromBody] ImageUploadDto imageUploadDto)
        {
            return await _mediaStoragesAppService.UploadImage(imageUploadDto);
        }
    }
}
