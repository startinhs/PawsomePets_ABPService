using Volo.Abp.Account;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.LanguageManagement;
using Volo.Saas.Host;
using Volo.FileManagement;
    using Volo.Chat;

namespace PawsomePets;

[DependsOn(
    typeof(PawsomePetsDomainSharedModule),
    typeof(AbpFeatureManagementApplicationContractsModule),
    typeof(AbpSettingManagementApplicationContractsModule),
    typeof(AbpIdentityApplicationContractsModule),
    typeof(AbpAccountPublicApplicationContractsModule),
    typeof(AbpAccountAdminApplicationContractsModule),
    typeof(SaasHostApplicationContractsModule),
    typeof(LanguageManagementApplicationContractsModule),
    typeof(FileManagementApplicationContractsModule),
    typeof(ChatApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationContractsModule)
)]
public class PawsomePetsApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PawsomePetsDtoExtensions.Configure();
    }
}
