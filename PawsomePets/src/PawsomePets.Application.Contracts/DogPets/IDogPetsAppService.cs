using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using PawsomePets.Shared;

namespace PawsomePets.DogPets
{
    public interface IDogPetsAppService : IApplicationService
    {
        Task<IRemoteStreamContent> GetFileAsync(GetFileInput input);

        Task<AppFileDescriptorDto> UploadFileAsync(IRemoteStreamContent input);

        Task<PagedResultDto<DogPetDto>> GetListAsync(GetDogPetsInput input);

        Task<DogPetDto> GetAsync(int id);

        Task DeleteAsync(int id);

        Task<DogPetDto> CreateAsync(DogPetCreateDto input);

        Task<DogPetDto> UpdateAsync(int id, DogPetUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(DogPetExcelDownloadDto input);
        Task DeleteByIdsAsync(List<int> dogpetIds);

        Task DeleteAllAsync(GetDogPetsInput input);
        Task<PawsomePets.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}