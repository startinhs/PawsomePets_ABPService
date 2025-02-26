namespace PawsomePets.DogPets
{
    public static class DogPetConsts
    {
        private const string DefaultSorting = "{0}ImageId asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "DogPet." : string.Empty);
        }

    }
}