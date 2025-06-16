using PawsomePets.AbpBlobContainers;
using System.Threading.Tasks;

namespace PawsomePets.MediaStorages
{
    public partial interface IMediaStoragesAppService
    {
        Task<object> GetImageByFileName(string fileName, bool isDownload);
        Task<object> UploadFile(FileUpload fileUpload);
        Task<object> DeleteFile(string fileName);
    }
}