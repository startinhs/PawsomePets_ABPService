using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace PawsomePets.DogPets
{
    public abstract class DogPetDtoBase : EntityDto<int>, IHasConcurrencyStamp
    {
        public string? Name { get; set; }
        public string? Breed { get; set; }
        public float Age { get; set; }
        public string? Gender { get; set; }
        public string? Color { get; set; }
        public float Weight { get; set; }
        public string? HealthStatus { get; set; }
        public int Vaccinations { get; set; }
        public decimal Price { get; set; }
        public float PromotionPecents { get; set; }
        public bool IsStock { get; set; }
        public string? OtherInformation { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}