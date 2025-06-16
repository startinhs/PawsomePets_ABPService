namespace PawsomePets.MediaStorages
{
    public static class MediaStorageConsts
    {
        private const string DefaultSorting = "{0}FileName asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "MediaStorage." : string.Empty);
        }

        public const int FileNameMaxLength = 50;
        public const int FileTypeMaxLength = 20;
        public const int ProviderNameMaxLength = 50;
        public const int ContainerNameMaxLength = 50;
        public const int EntityTypeMaxLength = 50;
    }
}