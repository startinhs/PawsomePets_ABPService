namespace PawsomePets.AbpBlobContainers
{
    public class ImageUpload
    {
        public string FileName { get; set; }
        public string ImageBytes { get; set; }
        public string ImageExtension { get; set; }
        public string Folder { get; set; }
    }
}
