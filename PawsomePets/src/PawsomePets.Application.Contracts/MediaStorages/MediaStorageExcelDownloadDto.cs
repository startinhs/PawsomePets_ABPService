using Volo.Abp.Application.Dtos;
using System;

namespace PawsomePets.MediaStorages
{
    public abstract class MediaStorageExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public string? Description { get; set; }
        public string? FileType { get; set; }
        public float? FileSizeMin { get; set; }
        public float? FileSizeMax { get; set; }
        public bool? IsMain { get; set; }
        public string? ProviderName { get; set; }
        public string? ContainerName { get; set; }
        public int? EntityIdMin { get; set; }
        public int? EntityIdMax { get; set; }
        public string? EntityType { get; set; }

        public MediaStorageExcelDownloadDtoBase()
        {

        }
    }
}