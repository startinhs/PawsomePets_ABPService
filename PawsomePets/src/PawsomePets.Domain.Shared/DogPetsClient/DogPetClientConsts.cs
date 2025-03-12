namespace PawsomePets.DogPetsClient
{
    public static class DogPetClientConsts
    {
        private const string DefaultSorting = "{0}ImageId asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "DogPetClient." : string.Empty);
        }

    }
}