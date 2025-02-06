using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using PawsomePets.Localization;

namespace PawsomePets.Blazor.Client;

public class PawsomePetsBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<PawsomePetsResource> _localizer;

    public PawsomePetsBrandingProvider(IStringLocalizer<PawsomePetsResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
