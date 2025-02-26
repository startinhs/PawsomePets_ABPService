using System;

namespace PawsomePets.DogPets;

[Serializable]
public class DogPetDownloadTokenCacheItem
{
    public string Token { get; set; } = null!;
}