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
using Microsoft.Extensions.Configuration;
using PawsomePets.AbpBlobContainers;
using Volo.Abp.BlobStoring;
using PawsomePets.Services.FileService;

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

        public async Task<object> UploadFile(FileUpload fileUpload)
        {
            try
            {
                var imageURL = "";
                var contentBytes = Convert.FromBase64String(fileUpload.FileBytes);
                var name = $"{fileUpload.FileName}.{fileUpload.FileExtension}";
                switch (_selectedBlobProvider)
                {
                    case "Azure":
                        {
                            await _azureContainer.SaveAsync(name, contentBytes, false);
                            var blob = await _azureContainer.GetAllBytesAsync(name);
                            imageURL = $"/image-azure/{name}";

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
                            imageURL = $"/image-aws/{name}";

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
                            var imageUrl = $"/image/{name}";

                            return new
                            {
                                imageUrl,
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