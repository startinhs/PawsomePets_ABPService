using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using PawsomePets.MongoDB;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace PawsomePets.DogPets
{
    public class MongoDogPetRepository : MongoDbRepository<PawsomePetsMongoDbContext, DogPet, int>, IDogPetRepository
    {
        public MongoDogPetRepository(IMongoDbContextProvider<PawsomePetsMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task DeleteAllAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, breed, ageMin, ageMax, gender, color, weightMin, weightMax, healthStatus, vaccinationsMin, vaccinationsMax, priceMin, priceMax, promotionPecentsMin, promotionPecentsMax, isStock, otherInformation);

            var ids = query.Select(x => x.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<DogPet>> GetListAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, breed, ageMin, ageMax, gender, color, weightMin, weightMax, healthStatus, vaccinationsMin, vaccinationsMax, priceMin, priceMax, promotionPecentsMin, promotionPecentsMax, isStock, otherInformation);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? DogPetConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<DogPet>>()
                .PageBy<DogPet, IMongoQueryable<DogPet>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, breed, ageMin, ageMax, gender, color, weightMin, weightMax, healthStatus, vaccinationsMin, vaccinationsMax, priceMin, priceMax, promotionPecentsMin, promotionPecentsMax, isStock, otherInformation);
            return await query.As<IMongoQueryable<DogPet>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<DogPet> ApplyFilter(
            IQueryable<DogPet> query,
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
            string? otherInformation = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name!.Contains(filterText!) || e.Breed!.Contains(filterText!) || e.Gender!.Contains(filterText!) || e.Color!.Contains(filterText!) || e.HealthStatus!.Contains(filterText!) || e.OtherInformation!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                    .WhereIf(!string.IsNullOrWhiteSpace(breed), e => e.Breed.Contains(breed))
                    .WhereIf(ageMin.HasValue, e => e.Age >= ageMin!.Value)
                    .WhereIf(ageMax.HasValue, e => e.Age <= ageMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(gender), e => e.Gender.Contains(gender))
                    .WhereIf(!string.IsNullOrWhiteSpace(color), e => e.Color.Contains(color))
                    .WhereIf(weightMin.HasValue, e => e.Weight >= weightMin!.Value)
                    .WhereIf(weightMax.HasValue, e => e.Weight <= weightMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(healthStatus), e => e.HealthStatus.Contains(healthStatus))
                    .WhereIf(vaccinationsMin.HasValue, e => e.Vaccinations >= vaccinationsMin!.Value)
                    .WhereIf(vaccinationsMax.HasValue, e => e.Vaccinations <= vaccinationsMax!.Value)
                    .WhereIf(priceMin.HasValue, e => e.Price >= priceMin!.Value)
                    .WhereIf(priceMax.HasValue, e => e.Price <= priceMax!.Value)
                    .WhereIf(promotionPecentsMin.HasValue, e => e.PromotionPecents >= promotionPecentsMin!.Value)
                    .WhereIf(promotionPecentsMax.HasValue, e => e.PromotionPecents <= promotionPecentsMax!.Value)
                    .WhereIf(isStock.HasValue, e => e.IsStock == isStock)
                    .WhereIf(!string.IsNullOrWhiteSpace(otherInformation), e => e.OtherInformation.Contains(otherInformation));
        }
    }
}