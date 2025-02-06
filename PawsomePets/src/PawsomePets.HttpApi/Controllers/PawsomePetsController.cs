using PawsomePets.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace PawsomePets.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class PawsomePetsController : AbpControllerBase
{
    protected PawsomePetsController()
    {
        LocalizationResource = typeof(PawsomePetsResource);
    }
}
