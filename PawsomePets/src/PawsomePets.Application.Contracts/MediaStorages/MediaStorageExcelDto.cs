using System;

namespace PawsomePets.MediaStorages
{
    public abstract class MediaStorageExcelDtoBase
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
    }
}