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

namespace PawsomePets.MediaStorages
{
    public abstract class MongoMediaStorageRepositoryBase : MongoDbRepository<PawsomePetsMongoDbContext, MediaStorage, int>
    {
        public MongoMediaStorageRepositoryBase(IMongoDbContextProvider<PawsomePetsMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? fileName = null,
            string? fileUrl = null,
            string? description = null,
            string? fileType = null,
            float? fileSizeMin = null,
            float? fileSizeMax = null,
            bool? isMain = null,
            string? providerName = null,
            string? containerName = null,
            int? entityIdMin = null,
            int? entityIdMax = null,
            string? entityType = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, fileName, fileUrl, description, fileType, fileSizeMin, fileSizeMax, isMain, providerName, containerName, entityIdMin, entityIdMax, entityType);

            var ids = query.Select(x => x.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<MediaStorage>> GetListAsync(
            string? filterText = null,
            string? fileName = null,
            string? fileUrl = null,
            string? description = null,
            string? fileType = null,
            float? fileSizeMin = null,
            float? fileSizeMax = null,
            bool? isMain = null,
            string? providerName = null,
            string? containerName = null,
            int? entityIdMin = null,
            int? entityIdMax = null,
            string? entityType = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, fileName, fileUrl, description, fileType, fileSizeMin, fileSizeMax, isMain, providerName, containerName, entityIdMin, entityIdMax, entityType);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MediaStorageConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<MediaStorage>>()
                .PageBy<MediaStorage, IMongoQueryable<MediaStorage>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? fileName = null,
            string? fileUrl = null,
            string? description = null,
            string? fileType = null,
            float? fileSizeMin = null,
            float? fileSizeMax = null,
            bool? isMain = null,
            string? providerName = null,
            string? containerName = null,
            int? entityIdMin = null,
            int? entityIdMax = null,
            string? entityType = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, fileName, fileUrl, description, fileType, fileSizeMin, fileSizeMax, isMain, providerName, containerName, entityIdMin, entityIdMax, entityType);
            return await query.As<IMongoQueryable<MediaStorage>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<MediaStorage> ApplyFilter(
            IQueryable<MediaStorage> query,
            string? filterText = null,
            string? fileName = null,
            string? fileUrl = null,
            string? description = null,
            string? fileType = null,
            float? fileSizeMin = null,
            float? fileSizeMax = null,
            bool? isMain = null,
            string? providerName = null,
            string? containerName = null,
            int? entityIdMin = null,
            int? entityIdMax = null,
            string? entityType = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.FileName!.Contains(filterText!) || e.FileUrl!.Contains(filterText!) || e.Description!.Contains(filterText!) || e.FileType!.Contains(filterText!) || e.ProviderName!.Contains(filterText!) || e.ContainerName!.Contains(filterText!) || e.EntityType!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(fileName), e => e.FileName.Contains(fileName))
                    .WhereIf(!string.IsNullOrWhiteSpace(fileUrl), e => e.FileUrl.Contains(fileUrl))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                    .WhereIf(!string.IsNullOrWhiteSpace(fileType), e => e.FileType.Contains(fileType))
                    .WhereIf(fileSizeMin.HasValue, e => e.FileSize >= fileSizeMin!.Value)
                    .WhereIf(fileSizeMax.HasValue, e => e.FileSize <= fileSizeMax!.Value)
                    .WhereIf(isMain.HasValue, e => e.IsMain == isMain)
                    .WhereIf(!string.IsNullOrWhiteSpace(providerName), e => e.ProviderName.Contains(providerName))
                    .WhereIf(!string.IsNullOrWhiteSpace(containerName), e => e.ContainerName.Contains(containerName))
                    .WhereIf(entityIdMin.HasValue, e => e.EntityId >= entityIdMin!.Value)
                    .WhereIf(entityIdMax.HasValue, e => e.EntityId <= entityIdMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(entityType), e => e.EntityType.Contains(entityType));
        }
    }
}