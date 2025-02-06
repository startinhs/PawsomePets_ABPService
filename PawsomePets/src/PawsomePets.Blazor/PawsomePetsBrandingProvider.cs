using Microsoft.Extensions.Localization;
using PawsomePets.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace PawsomePets.Blazor;

[Dependency(ReplaceServices = true)]
public class PawsomePetsBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<PawsomePetsResource> _localizer;

    public PawsomePetsBrandingProvider(IStringLocalizer<PawsomePetsResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
