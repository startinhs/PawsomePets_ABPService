using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace PawsomePets.MediaStorages
{
    public abstract class MediaStorageManagerBase : DomainService
    {
        protected IMediaStorageRepository _mediaStorageRepository;

        public MediaStorageManagerBase(IMediaStorageRepository mediaStorageRepository)
        {
            _mediaStorageRepository = mediaStorageRepository;
        }

        public virtual async Task<MediaStorage> CreateAsync(
        bool isMain, string? imageName = null, string? imageUrl = null, string? description = null, string? fileType = null, float? fileSize = null, string? providerName = null, string? containerName = null, int? entityId = null, string? entityType = null)
        {
            Check.Length(imageName, nameof(imageName), MediaStorageConsts.ImageNameMaxLength);
            Check.Length(fileType, nameof(fileType), MediaStorageConsts.FileTypeMaxLength);
            Check.Length(providerName, nameof(providerName), MediaStorageConsts.ProviderNameMaxLength);
            Check.Length(containerName, nameof(containerName), MediaStorageConsts.ContainerNameMaxLength);
            Check.Length(entityType, nameof(entityType), MediaStorageConsts.EntityTypeMaxLength);

            var mediaStorage = new MediaStorage(

             isMain, imageName, imageUrl, description, fileType, fileSize, providerName, containerName, entityId, entityType
             );

            return await _mediaStorageRepository.InsertAsync(mediaStorage);
        }

        public virtual async Task<MediaStorage> UpdateAsync(
            int id,
            bool isMain, string? imageName = null, string? imageUrl = null, string? description = null, string? fileType = null, float? fileSize = null, string? providerName = null, string? containerName = null, int? entityId = null, string? entityType = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.Length(imageName, nameof(imageName), MediaStorageConsts.ImageNameMaxLength);
            Check.Length(fileType, nameof(fileType), MediaStorageConsts.FileTypeMaxLength);
            Check.Length(providerName, nameof(providerName), MediaStorageConsts.ProviderNameMaxLength);
            Check.Length(containerName, nameof(containerName), MediaStorageConsts.ContainerNameMaxLength);
            Check.Length(entityType, nameof(entityType), MediaStorageConsts.EntityTypeMaxLength);

            var mediaStorage = await _mediaStorageRepository.GetAsync(id);

            mediaStorage.IsMain = isMain;
            mediaStorage.ImageName = imageName;
            mediaStorage.ImageUrl = imageUrl;
            mediaStorage.Description = description;
            mediaStorage.FileType = fileType;
            mediaStorage.FileSize = fileSize;
            mediaStorage.ProviderName = providerName;
            mediaStorage.ContainerName = containerName;
            mediaStorage.EntityId = entityId;
            mediaStorage.EntityType = entityType;

            mediaStorage.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _mediaStorageRepository.UpdateAsync(mediaStorage);
        }

    }
}