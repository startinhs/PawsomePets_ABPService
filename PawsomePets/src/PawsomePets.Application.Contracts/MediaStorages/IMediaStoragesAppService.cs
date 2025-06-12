using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using PawsomePets.Shared;

namespace PawsomePets.MediaStorages
{
    public partial interface IMediaStoragesAppService : IApplicationService
    {

        Task<PagedResultDto<MediaStorageDto>> GetListAsync(GetMediaStoragesInput input);

        Task<MediaStorageDto> GetAsync(int id);

        Task DeleteAsync(int id);

        Task<MediaStorageDto> CreateAsync(MediaStorageCreateDto input);

        Task<MediaStorageDto> UpdateAsync(int id, MediaStorageUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(MediaStorageExcelDownloadDto input);
        Task DeleteByIdsAsync(List<int> mediastorageIds);

        Task DeleteAllAsync(GetMediaStoragesInput input);
        Task<PawsomePets.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}