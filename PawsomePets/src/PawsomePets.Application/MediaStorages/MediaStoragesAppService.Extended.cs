using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using PawsomePets.Permissions;
using PawsomePets.MediaStorages;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using PawsomePets.Shared;
using PawsomePets.AbpBlobContainers;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.BlobStoring;

namespace PawsomePets.MediaStorages
{
    public class MediaStoragesAppService : MediaStoragesAppServiceBase, IMediaStoragesAppService
    {
        private readonly IBlobContainer<DatabaseContainer> _databaseContainer;
        private readonly IBlobContainer<AwsContainer> _awsContainer;
        private readonly IBlobContainer<AzureContainer> _azureContainer;
        public MediaStoragesAppService(IMediaStorageRepository mediaStorageRepository, MediaStorageManager mediaStorageManager, IDistributedCache<MediaStorageDownloadTokenCacheItem, string> downloadTokenCache, IBlobContainer<DatabaseContainer> databaseContainer, IBlobContainer<AwsContainer> awsContainer, IBlobContainer<AzureContainer> azureContainer)
            : base(mediaStorageRepository, mediaStorageManager, downloadTokenCache)
        {
            _databaseContainer = databaseContainer;
            _awsContainer = awsContainer;
            _azureContainer = azureContainer;
        }

        public async Task<object> UploadImage(AbpImageUploadDto imageUploadDto)
        {
            try { 
                var image = new ImageUpload
                {
                    ImageBytes = imageUploadDto.ImageBytes
                };

                return await _mediaStorageRepository.UploadImage(image);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}