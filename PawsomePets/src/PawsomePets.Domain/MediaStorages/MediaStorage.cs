using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities;

using Volo.Abp;

namespace PawsomePets.MediaStorages
{
    public abstract class MediaStorageBase : AuditedEntity<int>, IMultiTenant, IHasConcurrencyStamp
    {
        public virtual Guid? TenantId { get; set; }

        [CanBeNull]
        public virtual string? FileName { get; set; }

        [CanBeNull]
        public virtual string? FileUrl { get; set; }

        [CanBeNull]
        public virtual string? Description { get; set; }

        [CanBeNull]
        public virtual string? FileType { get; set; }

        public virtual float? FileSize { get; set; }

        public virtual bool IsMain { get; set; }

        [CanBeNull]
        public virtual string? ProviderName { get; set; }

        [CanBeNull]
        public virtual string? ContainerName { get; set; }

        public virtual int? EntityId { get; set; }

        [CanBeNull]
        public virtual string? EntityType { get; set; }

        public string ConcurrencyStamp { get; set; }

        protected MediaStorageBase()
        {

        }

        public MediaStorageBase(bool isMain, string? fileName = null, string? fileUrl = null, string? description = null, string? fileType = null, float? fileSize = null, string? providerName = null, string? containerName = null, int? entityId = null, string? entityType = null)
        {
            ConcurrencyStamp = Guid.NewGuid().ToString("N");

            Check.Length(fileName, nameof(fileName), MediaStorageConsts.FileNameMaxLength, 0);
            Check.Length(fileType, nameof(fileType), MediaStorageConsts.FileTypeMaxLength, 0);
            Check.Length(providerName, nameof(providerName), MediaStorageConsts.ProviderNameMaxLength, 0);
            Check.Length(containerName, nameof(containerName), MediaStorageConsts.ContainerNameMaxLength, 0);
            Check.Length(entityType, nameof(entityType), MediaStorageConsts.EntityTypeMaxLength, 0);
            IsMain = isMain;
            FileName = fileName;
            FileUrl = fileUrl;
            Description = description;
            FileType = fileType;
            FileSize = fileSize;
            ProviderName = providerName;
            ContainerName = containerName;
            EntityId = entityId;
            EntityType = entityType;
        }

    }
}