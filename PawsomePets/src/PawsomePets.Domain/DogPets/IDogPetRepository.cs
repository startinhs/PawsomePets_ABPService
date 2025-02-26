using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace PawsomePets.DogPets
{
    public interface IDogPetRepository : IRepository<DogPet, int>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? name = null,
            string? breed = null,
            float? ageMin = null,
            float? ageMax = null,
            string? gender = null,
            string? color = null,
            float? weightMin = null,
            float? weightMax = null,
            string? healthStatus = null,
            int? vaccinationsMin = null,
            int? vaccinationsMax = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            float? promotionPecentsMin = null,
            float? promotionPecentsMax = null,
            bool? isStock = null,
            string? otherInformation = null,
            CancellationToken cancellationToken = default);
        Task<List<DogPet>> GetListAsync(
                    string? filterText = null,
                    string? name = null,
                    string? breed = null,
                    float? ageMin = null,
                    float? ageMax = null,
                    string? gender = null,
                    string? color = null,
                    float? weightMin = null,
                    float? weightMax = null,
                    string? healthStatus = null,
                    int? vaccinationsMin = null,
                    int? vaccinationsMax = null,
                    decimal? priceMin = null,
                    decimal? priceMax = null,
                    float? promotionPecentsMin = null,
                    float? promotionPecentsMax = null,
                    bool? isStock = null,
                    string? otherInformation = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            string? breed = null,
            float? ageMin = null,
            float? ageMax = null,
            string? gender = null,
            string? color = null,
            float? weightMin = null,
            float? weightMax = null,
            string? healthStatus = null,
            int? vaccinationsMin = null,
            int? vaccinationsMax = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            float? promotionPecentsMin = null,
            float? promotionPecentsMax = null,
            bool? isStock = null,
            string? otherInformation = null,
            CancellationToken cancellationToken = default);
    }
}