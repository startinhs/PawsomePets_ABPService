namespace PawsomePets.AbpBlobContainers
{
    public class ImageUpload
    {
        public string FileName { get; set; }
        public string ImageBytes { get; set; }
        public string ImageExtension { get; set; }
        public string Folder { get; set; }
    }

    public class ImageUploadDto
    {
        public string ImageBytes { get; set; }
    }

    public class GetBlobRequestDto
    {
        public string Name { get; set; }
    }
}
