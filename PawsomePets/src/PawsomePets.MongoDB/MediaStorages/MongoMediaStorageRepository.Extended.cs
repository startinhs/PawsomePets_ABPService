using System;
using System.Threading.Tasks;
using PawsomePets.MongoDB;
using Volo.Abp.MongoDB;
using Microsoft.Extensions.Configuration;
using PawsomePets.AbpBlobContainers;
using Volo.Abp.BlobStoring;
using PawsomePets.Services.FileService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace PawsomePets.MediaStorages
{
    public class MongoMediaStorageRepository : MongoMediaStorageRepositoryBase, IMediaStorageRepository
    {
        private readonly string _selectedBlobProvider;
        protected IConfiguration _configuration;
        protected IFileService _fileService;
        private readonly IBlobContainer<DatabaseContainer> _databaseContainer;
        private readonly IBlobContainer<FileSystemContainer> _fileSystemContainer;
        private readonly IBlobContainer<AwsContainer> _awsContainer;
        private readonly IBlobContainer<AzureContainer> _azureContainer;
        public MongoMediaStorageRepository(IMongoDbContextProvider<PawsomePetsMongoDbContext> dbContextProvider,
            IConfiguration configuration, IFileService fileService, IBlobContainer<DatabaseContainer> databaseContainer, IBlobContainer<FileSystemContainer> fileSystemContainer, IBlobContainer<AwsContainer> awsContainer, IBlobContainer<AzureContainer> azureContainer)
            : base(dbContextProvider)
        {
            _configuration = configuration;
            _selectedBlobProvider = _configuration.GetValue<string>("SelectedBlobProvider");
            _fileService = fileService;
            _databaseContainer = databaseContainer;
            _fileSystemContainer = fileSystemContainer;
            _awsContainer = awsContainer;
            _azureContainer = azureContainer;
        }

        public async Task<object> GetImageByFileName(string fileName, bool isDownload)
        {
            try
            {
                byte[] blob = _selectedBlobProvider switch
                {
                    "Azure" => await _azureContainer.GetAllBytesAsync(fileName),
                    "AmazonS3" => await _awsContainer.GetAllBytesAsync(fileName),
                    "Database" => await _databaseContainer.GetAllBytesAsync(fileName),
                    _ => throw new Exception($"Unsupported provider: {_selectedBlobProvider}")
                };

                var contentType = GetContentType(fileName);

                if (isDownload)
                {
                    return new FileContentResult(blob, contentType)
                    {
                        FileDownloadName = fileName
                    };
                }

                return new FileContentResult(blob, contentType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get file error: {ex.Message}");
                throw new Exception("Unable to get file.");
            }
        }

        private string GetContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            return provider.TryGetContentType(fileName, out var contentType)
                ? contentType
                : "application/octet-stream";
        }

        public async Task<object> UploadFile(FileUpload fileUpload)
        {
            try
            {
                
                var contentBytes = Convert.FromBase64String(fileUpload.FileBytes);
                var name = $"{fileUpload.FileName}.{fileUpload.FileExtension}";
                var imageURL = $"/media/{name}";
                switch (_selectedBlobProvider)
                {
                    case "Azure":
                        {
                            await _azureContainer.SaveAsync(name, contentBytes, false);
                            var blob = await _azureContainer.GetAllBytesAsync(name);
                            return new
                            {
                                imageUrl = imageURL,
                                imageName = name
                            };
                        }

                    case "AmazonS3":
                        {
                            await _awsContainer.SaveAsync(name, contentBytes, false);
                            var blob = await _awsContainer.GetAllBytesAsync(name);
                            return new
                            {
                                imageUrl = imageURL,
                                imageName = name
                            };
                        }
                    case "Database":
                        {
                            await _databaseContainer.SaveAsync(name, contentBytes, overrideExisting: false);
                            var blob = await _databaseContainer.GetAllBytesAsync(name);
                            return new
                            {
                                imageUrl = imageURL,
                                imageName = name
                            };
                        }
                    case "FileSystem":
                        {   
                            var path = _configuration.GetValue<string>("FileSystemPath");
                            await _fileSystemContainer.SaveAsync(name, contentBytes, false);
                            imageURL = $"/{path.Substring(path.IndexOf('/') + 1)}/host/file-system/{name}";

                            return new
                            {
                                imageUrl = imageURL,
                                imageName = name,
                            };
                        }
                }
                var _dbContext = await GetDbContextAsync();

                throw new Exception($"File upload failed from provider: {_selectedBlobProvider}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during file upload: {ex.Message}");
                throw new Exception("File upload failed from service");
            }
        }

        public async Task<object> DeleteFile(string fileName)
        {
            try
            {
                switch (_selectedBlobProvider)
                {
                    case "Azure":
                        return await _azureContainer.DeleteAsync(fileName);
                    case "AmazonS3":
                        return await _awsContainer.DeleteAsync(fileName);
                    case "Database":
                        return await _databaseContainer.DeleteAsync(fileName);
                    case "FileSystem":
                        return await _fileSystemContainer.DeleteAsync(fileName);
                }
                throw new Exception("Image delete failed");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return true;
        }
    }
}