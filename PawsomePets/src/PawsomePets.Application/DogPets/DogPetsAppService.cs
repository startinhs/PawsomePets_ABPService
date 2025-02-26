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
using PawsomePets.DogPets;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using PawsomePets.Shared;
using Volo.Abp.BlobStoring;

namespace PawsomePets.DogPets
{

    [Authorize(PawsomePetsPermissions.DogPets.Default)]
    public class DogPetsAppService : PawsomePetsAppService, IDogPetsAppService
    {
        protected IDistributedCache<DogPetDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IDogPetRepository _dogPetRepository;
        protected DogPetManager _dogPetManager;
        protected IRepository<AppFileDescriptors.AppFileDescriptor, Guid> _appFileDescriptorRepository;
        protected IBlobContainer<DogPetFileContainer> _blobContainer;

        public DogPetsAppService(IDogPetRepository dogPetRepository, DogPetManager dogPetManager, IDistributedCache<DogPetDownloadTokenCacheItem, string> downloadTokenCache, IRepository<AppFileDescriptors.AppFileDescriptor, Guid> appFileDescriptorRepository, IBlobContainer<DogPetFileContainer> blobContainer)
        {
            _downloadTokenCache = downloadTokenCache;
            _dogPetRepository = dogPetRepository;
            _dogPetManager = dogPetManager;
            _appFileDescriptorRepository = appFileDescriptorRepository;
            _blobContainer = blobContainer;
        }

        public virtual async Task<PagedResultDto<DogPetDto>> GetListAsync(GetDogPetsInput input)
        {
            var totalCount = await _dogPetRepository.GetCountAsync(input.FilterText, input.Name, input.Breed, input.AgeMin, input.AgeMax, input.Gender, input.Color, input.WeightMin, input.WeightMax, input.HealthStatus, input.VaccinationsMin, input.VaccinationsMax, input.PriceMin, input.PriceMax, input.PromotionPecentsMin, input.PromotionPecentsMax, input.IsStock, input.OtherInformation);
            var items = await _dogPetRepository.GetListAsync(input.FilterText, input.Name, input.Breed, input.AgeMin, input.AgeMax, input.Gender, input.Color, input.WeightMin, input.WeightMax, input.HealthStatus, input.VaccinationsMin, input.VaccinationsMax, input.PriceMin, input.PriceMax, input.PromotionPecentsMin, input.PromotionPecentsMax, input.IsStock, input.OtherInformation, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<DogPetDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<DogPet>, List<DogPetDto>>(items)
            };
        }

        public virtual async Task<DogPetDto> GetAsync(int id)
        {
            return ObjectMapper.Map<DogPet, DogPetDto>(await _dogPetRepository.GetAsync(id));
        }

        [Authorize(PawsomePetsPermissions.DogPets.Delete)]
        public virtual async Task DeleteAsync(int id)
        {
            await _dogPetRepository.DeleteAsync(id);
        }

        [Authorize(PawsomePetsPermissions.DogPets.Create)]
        public virtual async Task<DogPetDto> CreateAsync(DogPetCreateDto input)
        {

            var dogPet = await _dogPetManager.CreateAsync(
            input.ImageId, input.Age, input.Weight, input.Vaccinations, input.Price, input.PromotionPecents, input.IsStock, input.Name, input.Breed, input.Gender, input.Color, input.HealthStatus, input.OtherInformation
            );

            return ObjectMapper.Map<DogPet, DogPetDto>(dogPet);
        }

        [Authorize(PawsomePetsPermissions.DogPets.Edit)]
        public virtual async Task<DogPetDto> UpdateAsync(int id, DogPetUpdateDto input)
        {

            var dogPet = await _dogPetManager.UpdateAsync(
            id,
            input.ImageId, input.Age, input.Weight, input.Vaccinations, input.Price, input.PromotionPecents, input.IsStock, input.Name, input.Breed, input.Gender, input.Color, input.HealthStatus, input.OtherInformation, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<DogPet, DogPetDto>(dogPet);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(DogPetExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _dogPetRepository.GetListAsync(input.FilterText, input.Name, input.Breed, input.AgeMin, input.AgeMax, input.Gender, input.Color, input.WeightMin, input.WeightMax, input.HealthStatus, input.VaccinationsMin, input.VaccinationsMax, input.PriceMin, input.PriceMax, input.PromotionPecentsMin, input.PromotionPecentsMax, input.IsStock, input.OtherInformation);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<DogPet>, List<DogPetExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "DogPets.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(PawsomePetsPermissions.DogPets.Delete)]
        public virtual async Task DeleteByIdsAsync(List<int> dogpetIds)
        {
            await _dogPetRepository.DeleteManyAsync(dogpetIds);
        }

        [Authorize(PawsomePetsPermissions.DogPets.Delete)]
        public virtual async Task DeleteAllAsync(GetDogPetsInput input)
        {
            await _dogPetRepository.DeleteAllAsync(input.FilterText, input.Name, input.Breed, input.AgeMin, input.AgeMax, input.Gender, input.Color, input.WeightMin, input.WeightMax, input.HealthStatus, input.VaccinationsMin, input.VaccinationsMax, input.PriceMin, input.PriceMax, input.PromotionPecentsMin, input.PromotionPecentsMax, input.IsStock, input.OtherInformation);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetFileAsync(GetFileInput input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var fileDescriptor = await _appFileDescriptorRepository.GetAsync(input.FileId);
            var stream = await _blobContainer.GetAsync(fileDescriptor.Id.ToString("N"));

            return new RemoteStreamContent(stream, fileDescriptor.Name, fileDescriptor.MimeType);
        }

        public virtual async Task<AppFileDescriptorDto> UploadFileAsync(IRemoteStreamContent input)
        {
            var id = GuidGenerator.Create();
            var fileDescriptor = await _appFileDescriptorRepository.InsertAsync(new AppFileDescriptors.AppFileDescriptor(id, input.FileName, input.ContentType));

            await _blobContainer.SaveAsync(fileDescriptor.Id.ToString("N"), input.GetStream());

            return ObjectMapper.Map<AppFileDescriptors.AppFileDescriptor, AppFileDescriptorDto>(fileDescriptor);
        }

        public virtual async Task<PawsomePets.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new DogPetDownloadTokenCacheItem { Token = token },
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