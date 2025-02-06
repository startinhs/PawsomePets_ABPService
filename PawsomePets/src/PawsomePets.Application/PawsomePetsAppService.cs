using PawsomePets.Localization;
using Volo.Abp.Application.Services;

namespace PawsomePets;

/* Inherit your application services from this class.
 */
public abstract class PawsomePetsAppService : ApplicationService
{
    protected PawsomePetsAppService()
    {
        LocalizationResource = typeof(PawsomePetsResource);
    }
}
