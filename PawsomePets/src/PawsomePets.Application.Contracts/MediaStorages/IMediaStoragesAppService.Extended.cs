using PawsomePets.AbpBlobContainers;
using System.Threading.Tasks;

namespace PawsomePets.MediaStorages
{
    public partial interface IMediaStoragesAppService
    {
        Task<object> UploadImage(ImageUploadDto imageUploadDto);
    }
}