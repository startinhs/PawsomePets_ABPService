using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using PawsomePets.Shared;

namespace PawsomePets.DogPetsClient
{
    public interface IDogPetsClientAppService : IApplicationService
    {
        Task<IRemoteStreamContent> GetFileAsync(GetFileInput input);

        Task<AppFileDescriptorDto> UploadFileAsync(IRemoteStreamContent input);

        Task<PagedResultDto<DogPetClientDto>> GetListAsync(GetDogPetsClientInput input);

        Task<DogPetClientDto> GetAsync(int id);

        Task DeleteAsync(int id);

        Task<DogPetClientDto> CreateAsync(DogPetClientCreateDto input);

        Task<DogPetClientDto> UpdateAsync(int id, DogPetClientUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(DogPetClientExcelDownloadDto input);
        Task DeleteByIdsAsync(List<int> dogpetclientIds);

        Task DeleteAllAsync(GetDogPetsClientInput input);
        Task<PawsomePets.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}