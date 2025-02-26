using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities;

using Volo.Abp;

namespace PawsomePets.DogPets
{
    public class DogPet : Entity<int>, IHasConcurrencyStamp
    {
        public virtual Guid ImageId { get; set; }

        [CanBeNull]
        public virtual string? Name { get; set; }

        [CanBeNull]
        public virtual string? Breed { get; set; }

        public virtual float Age { get; set; }

        [CanBeNull]
        public virtual string? Gender { get; set; }

        [CanBeNull]
        public virtual string? Color { get; set; }

        public virtual float Weight { get; set; }

        [CanBeNull]
        public virtual string? HealthStatus { get; set; }

        public virtual int Vaccinations { get; set; }

        public virtual decimal Price { get; set; }

        public virtual float PromotionPecents { get; set; }

        public virtual bool IsStock { get; set; }

        [CanBeNull]
        public virtual string? OtherInformation { get; set; }

        public string ConcurrencyStamp { get; set; }

        protected DogPet()
        {

        }

        public DogPet(Guid imageId, float age, float weight, int vaccinations, decimal price, float promotionPecents, bool isStock, string? name = null, string? breed = null, string? gender = null, string? color = null, string? healthStatus = null, string? otherInformation = null)
        {
            ConcurrencyStamp = Guid.NewGuid().ToString("N");

            ImageId = imageId;
            Age = age;
            Weight = weight;
            Vaccinations = vaccinations;
            Price = price;
            PromotionPecents = promotionPecents;
            IsStock = isStock;
            Name = name;
            Breed = breed;
            Gender = gender;
            Color = color;
            HealthStatus = healthStatus;
            OtherInformation = otherInformation;
        }

    }
}