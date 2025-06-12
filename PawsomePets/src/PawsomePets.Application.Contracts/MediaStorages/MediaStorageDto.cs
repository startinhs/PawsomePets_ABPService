using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace PawsomePets.MediaStorages
{
    public abstract class MediaStorageDtoBase : AuditedEntityDto<int>, IHasConcurrencyStamp
    {
        public string? ImageName { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public string? FileType { get; set; }
        public float? FileSize { get; set; }
        public bool IsMain { get; set; }
        public string? ProviderName { get; set; }
        public string? ContainerName { get; set; }
        public int? EntityId { get; set; }
        public string? EntityType { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}