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
        public async Task<object> GetBlob(string name)
        {
            return await _mediaStoragesAppService.GetBlob(name);
        }

        [Route("image-aws/{name}")]
        [HttpGet]
        public async Task<object> GetBlobAws(string name)
        {
            return await _mediaStoragesAppService.GetBlobAws(name);
        }

        [Route("image-azure/{name}")]
        [HttpGet]
        public async Task<object> GetBlobAzure(string name)
        {
            return await _mediaStoragesAppService.GetBlobAzure(name);
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
