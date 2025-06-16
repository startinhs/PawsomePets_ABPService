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

namespace PawsomePets.MediaStorages
{

    [Authorize(PawsomePetsPermissions.MediaStorages.Default)]
    public abstract class MediaStoragesAppServiceBase : PawsomePetsAppService
    {
        protected IDistributedCache<MediaStorageDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IMediaStorageRepository _mediaStorageRepository;
        protected MediaStorageManager _mediaStorageManager;

        public MediaStoragesAppServiceBase(IMediaStorageRepository mediaStorageRepository, MediaStorageManager mediaStorageManager, IDistributedCache<MediaStorageDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _mediaStorageRepository = mediaStorageRepository;
            _mediaStorageManager = mediaStorageManager;

        }

        public virtual async Task<PagedResultDto<MediaStorageDto>> GetListAsync(GetMediaStoragesInput input)
        {
            var totalCount = await _mediaStorageRepository.GetCountAsync(input.FilterText, input.FileName, input.FileUrl, input.Description, input.FileType, input.FileSizeMin, input.FileSizeMax, input.IsMain, input.ProviderName, input.ContainerName, input.EntityIdMin, input.EntityIdMax, input.EntityType);
            var items = await _mediaStorageRepository.GetListAsync(input.FilterText, input.FileName, input.FileUrl, input.Description, input.FileType, input.FileSizeMin, input.FileSizeMax, input.IsMain, input.ProviderName, input.ContainerName, input.EntityIdMin, input.EntityIdMax, input.EntityType, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<MediaStorageDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<MediaStorage>, List<MediaStorageDto>>(items)
            };
        }

        public virtual async Task<MediaStorageDto> GetAsync(int id)
        {
            return ObjectMapper.Map<MediaStorage, MediaStorageDto>(await _mediaStorageRepository.GetAsync(id));
        }

        [Authorize(PawsomePetsPermissions.MediaStorages.Delete)]
        public virtual async Task DeleteAsync(int id)
        {
            await _mediaStorageRepository.DeleteAsync(id);
        }

        [Authorize(PawsomePetsPermissions.MediaStorages.Create)]
        public virtual async Task<MediaStorageDto> CreateAsync(MediaStorageCreateDto input)
        {

            var mediaStorage = await _mediaStorageManager.CreateAsync(
            input.IsMain, input.FileName, input.FileUrl, input.Description, input.FileType, input.FileSize, input.ProviderName, input.ContainerName, input.EntityId, input.EntityType
            );

            return ObjectMapper.Map<MediaStorage, MediaStorageDto>(mediaStorage);
        }

        [Authorize(PawsomePetsPermissions.MediaStorages.Edit)]
        public virtual async Task<MediaStorageDto> UpdateAsync(int id, MediaStorageUpdateDto input)
        {

            var mediaStorage = await _mediaStorageManager.UpdateAsync(
            id,
            input.IsMain, input.FileName, input.FileUrl, input.Description, input.FileType, input.FileSize, input.ProviderName, input.ContainerName, input.EntityId, input.EntityType, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<MediaStorage, MediaStorageDto>(mediaStorage);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(MediaStorageExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _mediaStorageRepository.GetListAsync(input.FilterText, input.FileName, input.FileUrl, input.Description, input.FileType, input.FileSizeMin, input.FileSizeMax, input.IsMain, input.ProviderName, input.ContainerName, input.EntityIdMin, input.EntityIdMax, input.EntityType);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<MediaStorage>, List<MediaStorageExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "MediaStorages.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(PawsomePetsPermissions.MediaStorages.Delete)]
        public virtual async Task DeleteByIdsAsync(List<int> mediastorageIds)
        {
            await _mediaStorageRepository.DeleteManyAsync(mediastorageIds);
        }

        [Authorize(PawsomePetsPermissions.MediaStorages.Delete)]
        public virtual async Task DeleteAllAsync(GetMediaStoragesInput input)
        {
            await _mediaStorageRepository.DeleteAllAsync(input.FilterText, input.FileName, input.FileUrl, input.Description, input.FileType, input.FileSizeMin, input.FileSizeMax, input.IsMain, input.ProviderName, input.ContainerName, input.EntityIdMin, input.EntityIdMax, input.EntityType);
        }
        public virtual async Task<PawsomePets.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new MediaStorageDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new PawsomePets.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}