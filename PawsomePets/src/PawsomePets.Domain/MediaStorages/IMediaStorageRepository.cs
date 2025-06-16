using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace PawsomePets.MediaStorages
{
    public partial interface IMediaStorageRepository : IRepository<MediaStorage, int>
    {

        Task DeleteAllAsync(
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
            CancellationToken cancellationToken = default);
        Task<List<MediaStorage>> GetListAsync(
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
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default);
    }
}