using PawsomePets.AbpBlobContainers;
using System.Threading.Tasks;

namespace PawsomePets.MediaStorages
{
    public partial interface IMediaStoragesAppService
    {
        Task<object> GetBlob(string name);
        Task<object> GetBlobAws(string name);
        Task<object> GetBlobAzure(string name);
        Task<object> UploadImage(ImageUploadDto imageUploadDto);
        Task<object> DeleteFile(string fileName);
    }
}