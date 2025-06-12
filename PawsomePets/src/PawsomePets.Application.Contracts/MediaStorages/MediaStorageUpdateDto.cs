using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace PawsomePets.MediaStorages
{
    public abstract class MediaStorageUpdateDtoBase : IHasConcurrencyStamp
    {
        [StringLength(MediaStorageConsts.ImageNameMaxLength)]
        public string? ImageName { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        [StringLength(MediaStorageConsts.FileTypeMaxLength)]
        public string? FileType { get; set; }
        public float? FileSize { get; set; }
        public bool IsMain { get; set; }
        [StringLength(MediaStorageConsts.ProviderNameMaxLength)]
        public string? ProviderName { get; set; }
        [StringLength(MediaStorageConsts.ContainerNameMaxLength)]
        public string? ContainerName { get; set; }
        public int? EntityId { get; set; }
        [StringLength(MediaStorageConsts.EntityTypeMaxLength)]
        public string? EntityType { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}