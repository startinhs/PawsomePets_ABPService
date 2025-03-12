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
using PawsomePets.DogPetsClient;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using PawsomePets.Shared;
using Volo.Abp.BlobStoring;

namespace PawsomePets.DogPetsClient
{

    [Authorize(PawsomePetsPermissions.DogPetsClient.Default)]
    public class DogPetsClientAppService : PawsomePetsAppService, IDogPetsClientAppService
    {
        protected IDistributedCache<DogPetClientDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IDogPetClientRepository _dogPetClientRepository;
        protected DogPetClientManager _dogPetClientManager;
        protected IRepository<AppFileDescriptors.AppFileDescriptor, Guid> _appFileDescriptorRepository;
        protected IBlobContainer<DogPetClientFileContainer> _blobContainer;

        public DogPetsClientAppService(IDogPetClientRepository dogPetClientRepository, DogPetClientManager dogPetClientManager, IDistributedCache<DogPetClientDownloadTokenCacheItem, string> downloadTokenCache, IRepository<AppFileDescriptors.AppFileDescriptor, Guid> appFileDescriptorRepository, IBlobContainer<DogPetClientFileContainer> blobContainer)
        {
            _downloadTokenCache = downloadTokenCache;
            _dogPetClientRepository = dogPetClientRepository;
            _dogPetClientManager = dogPetClientManager;
            _appFileDescriptorRepository = appFileDescriptorRepository;
            _blobContainer = blobContainer;
        }

        public virtual async Task<PagedResultDto<DogPetClientDto>> GetListAsync(GetDogPetsClientInput input)
        {
            var totalCount = await _dogPetClientRepository.GetCountAsync(input.FilterText, input.Name, input.Breed, input.AgeMin, input.AgeMax, input.Gender, input.Color, input.WeightMin, input.WeightMax, input.HealthStatus, input.VaccinationsMin, input.VaccinationsMax, input.PriceMin, input.PriceMax, input.PromotionPecentsMin, input.PromotionPecentsMax, input.IsStock, input.OtherInformation);
            var items = await _dogPetClientRepository.GetListAsync(input.FilterText, input.Name, input.Breed, input.AgeMin, input.AgeMax, input.Gender, input.Color, input.WeightMin, input.WeightMax, input.HealthStatus, input.VaccinationsMin, input.VaccinationsMax, input.PriceMin, input.PriceMax, input.PromotionPecentsMin, input.PromotionPecentsMax, input.IsStock, input.OtherInformation, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<DogPetClientDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<DogPetClient>, List<DogPetClientDto>>(items)
            };
        }

        public virtual async Task<DogPetClientDto> GetAsync(int id)
        {
            return ObjectMapper.Map<DogPetClient, DogPetClientDto>(await _dogPetClientRepository.GetAsync(id));
        }

        [Authorize(PawsomePetsPermissions.DogPetsClient.Delete)]
        public virtual async Task DeleteAsync(int id)
        {
            await _dogPetClientRepository.DeleteAsync(id);
        }

        [Authorize(PawsomePetsPermissions.DogPetsClient.Create)]
        public virtual async Task<DogPetClientDto> CreateAsync(DogPetClientCreateDto input)
        {

            var dogPetClient = await _dogPetClientManager.CreateAsync(
            input.ImageId, input.Age, input.Weight, input.Vaccinations, input.Price, input.PromotionPecents, input.IsStock, input.Name, input.Breed, input.Gender, input.Color, input.HealthStatus, input.OtherInformation
            );

            return ObjectMapper.Map<DogPetClient, DogPetClientDto>(dogPetClient);
        }

        [Authorize(PawsomePetsPermissions.DogPetsClient.Edit)]
        public virtual async Task<DogPetClientDto> UpdateAsync(int id, DogPetClientUpdateDto input)
        {

            var dogPetClient = await _dogPetClientManager.UpdateAsync(
            id,
            input.ImageId, input.Age, input.Weight, input.Vaccinations, input.Price, input.PromotionPecents, input.IsStock, input.Name, input.Breed, input.Gender, input.Color, input.HealthStatus, input.OtherInformation, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<DogPetClient, DogPetClientDto>(dogPetClient);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(DogPetClientExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _dogPetClientRepository.GetListAsync(input.FilterText, input.Name, input.Breed, input.AgeMin, input.AgeMax, input.Gender, input.Color, input.WeightMin, input.WeightMax, input.HealthStatus, input.VaccinationsMin, input.VaccinationsMax, input.PriceMin, input.PriceMax, input.PromotionPecentsMin, input.PromotionPecentsMax, input.IsStock, input.OtherInformation);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<DogPetClient>, List<DogPetClientExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "DogPetsClient.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(PawsomePetsPermissions.DogPetsClient.Delete)]
        public virtual async Task DeleteByIdsAsync(List<int> dogpetclientIds)
        {
            await _dogPetClientRepository.DeleteManyAsync(dogpetclientIds);
        }

        [Authorize(PawsomePetsPermissions.DogPetsClient.Delete)]
        public virtual async Task DeleteAllAsync(GetDogPetsClientInput input)
        {
            await _dogPetClientRepository.DeleteAllAsync(input.FilterText, input.Name, input.Breed, input.AgeMin, input.AgeMax, input.Gender, input.Color, input.WeightMin, input.WeightMax, input.HealthStatus, input.VaccinationsMin, input.VaccinationsMax, input.PriceMin, input.PriceMax, input.PromotionPecentsMin, input.PromotionPecentsMax, input.IsStock, input.OtherInformation);
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
                new DogPetClientDownloadTokenCacheItem { Token = token },
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