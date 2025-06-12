using System;

namespace PawsomePets.MediaStorages;

public abstract class MediaStorageDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}