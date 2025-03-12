using System;

namespace PawsomePets.DogPetsClient;

[Serializable]
public class DogPetClientDownloadTokenCacheItem
{
    public string Token { get; set; } = null!;
}