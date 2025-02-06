using PawsomePets.Localization;
using Volo.Abp.AspNetCore.Components;

namespace PawsomePets.Blazor;

public abstract class PawsomePetsComponentBase : AbpComponentBase
{
    protected PawsomePetsComponentBase()
    {
        LocalizationResource = typeof(PawsomePetsResource);
    }
}
