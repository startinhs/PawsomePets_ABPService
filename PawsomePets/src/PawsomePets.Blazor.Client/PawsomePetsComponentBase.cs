using PawsomePets.Localization;
using Volo.Abp.AspNetCore.Components;

namespace PawsomePets.Blazor.Client;

public abstract class PawsomePetsComponentBase : AbpComponentBase
{
    protected PawsomePetsComponentBase()
    {
        LocalizationResource = typeof(PawsomePetsResource);
    }
}
