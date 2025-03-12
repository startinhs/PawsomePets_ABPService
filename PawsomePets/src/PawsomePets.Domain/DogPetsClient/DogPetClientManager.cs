using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace PawsomePets.DogPetsClient
{
    public class DogPetClientManager : DomainService
    {
        protected IDogPetClientRepository _dogPetClientRepository;

        public DogPetClientManager(IDogPetClientRepository dogPetClientRepository)
        {
            _dogPetClientRepository = dogPetClientRepository;
        }

        public virtual async Task<DogPetClient> CreateAsync(
        Guid imageId, float age, float weight, int vaccinations, decimal price, float promotionPecents, bool isStock, string? name = null, string? breed = null, string? gender = null, string? color = null, string? healthStatus = null, string? otherInformation = null)
        {

            var dogPetClient = new DogPetClient(

             imageId, age, weight, vaccinations, price, promotionPecents, isStock, name, breed, gender, color, healthStatus, otherInformation
             );

            return await _dogPetClientRepository.InsertAsync(dogPetClient);
        }

        public virtual async Task<DogPetClient> UpdateAsync(
            int id,
            Guid imageId, float age, float weight, int vaccinations, decimal price, float promotionPecents, bool isStock, string? name = null, string? breed = null, string? gender = null, string? color = null, string? healthStatus = null, string? otherInformation = null, [CanBeNull] string? concurrencyStamp = null
        )
        {

            var dogPetClient = await _dogPetClientRepository.GetAsync(id);

            dogPetClient.ImageId = imageId;
            dogPetClient.Age = age;
            dogPetClient.Weight = weight;
            dogPetClient.Vaccinations = vaccinations;
            dogPetClient.Price = price;
            dogPetClient.PromotionPecents = promotionPecents;
            dogPetClient.IsStock = isStock;
            dogPetClient.Name = name;
            dogPetClient.Breed = breed;
            dogPetClient.Gender = gender;
            dogPetClient.Color = color;
            dogPetClient.HealthStatus = healthStatus;
            dogPetClient.OtherInformation = otherInformation;

            dogPetClient.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _dogPetClientRepository.UpdateAsync(dogPetClient);
        }

    }
}