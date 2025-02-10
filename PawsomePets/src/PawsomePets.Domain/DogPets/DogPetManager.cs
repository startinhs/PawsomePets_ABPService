using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace PawsomePets.DogPets
{
    public abstract class DogPetManagerBase : DomainService
    {
        protected IDogPetRepository _dogPetRepository;

        public DogPetManagerBase(IDogPetRepository dogPetRepository)
        {
            _dogPetRepository = dogPetRepository;
        }

        public virtual async Task<DogPet> CreateAsync(
        float age, float weight, int vaccinations, decimal price, float promotionPecents, bool isStock, string? name = null, string? breed = null, string? gender = null, string? color = null, string? healthStatus = null, string? otherInformation = null)
        {

            var dogPet = new DogPet(

             age, weight, vaccinations, price, promotionPecents, isStock, name, breed, gender, color, healthStatus, otherInformation
             );

            return await _dogPetRepository.InsertAsync(dogPet);
        }

        public virtual async Task<DogPet> UpdateAsync(
            int id,
            float age, float weight, int vaccinations, decimal price, float promotionPecents, bool isStock, string? name = null, string? breed = null, string? gender = null, string? color = null, string? healthStatus = null, string? otherInformation = null, [CanBeNull] string? concurrencyStamp = null
        )
        {

            var dogPet = await _dogPetRepository.GetAsync(id);

            dogPet.Age = age;
            dogPet.Weight = weight;
            dogPet.Vaccinations = vaccinations;
            dogPet.Price = price;
            dogPet.PromotionPecents = promotionPecents;
            dogPet.IsStock = isStock;
            dogPet.Name = name;
            dogPet.Breed = breed;
            dogPet.Gender = gender;
            dogPet.Color = color;
            dogPet.HealthStatus = healthStatus;
            dogPet.OtherInformation = otherInformation;

            dogPet.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _dogPetRepository.UpdateAsync(dogPet);
        }

    }
}