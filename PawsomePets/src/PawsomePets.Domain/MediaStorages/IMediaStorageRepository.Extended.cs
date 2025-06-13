using PawsomePets.AbpBlobContainers;
using System.Threading.Tasks;

namespace PawsomePets.MediaStorages
{
    public partial interface IMediaStorageRepository
    {
        Task<object> UploadImage(ImageUpload image);
    }
}