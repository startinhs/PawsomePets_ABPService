using PawsomePets.DogPetsClient;
using PawsomePets.DogPets;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundJobs.MongoDB;
using Volo.Abp.FeatureManagement.MongoDB;
using Volo.Abp.Identity.MongoDB;
using Volo.Abp.OpenIddict.MongoDB;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.MongoDB;
using Volo.Abp.SettingManagement.MongoDB;
using Volo.Abp.BlobStoring.Database.MongoDB;
using Volo.Abp.Uow;
using Volo.Saas.MongoDB;
using Volo.Abp.LanguageManagement.MongoDB;
using Volo.FileManagement.MongoDB;
using Volo.Chat.MongoDB;

namespace PawsomePets.MongoDB;

[DependsOn(
    typeof(PawsomePetsDomainModule),
    typeof(AbpPermissionManagementMongoDbModule),
    typeof(AbpSettingManagementMongoDbModule),
    typeof(AbpBackgroundJobsMongoDbModule),
    typeof(AbpFeatureManagementMongoDbModule),
    typeof(AbpIdentityProMongoDbModule),
    typeof(AbpOpenIddictProMongoDbModule),
    typeof(LanguageManagementMongoDbModule),
    typeof(FileManagementMongoDbModule),
    typeof(SaasMongoDbModule),
    typeof(ChatMongoDbModule),
    typeof(BlobStoringDatabaseMongoDbModule)
)]
public class PawsomePetsMongoDbModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMongoDbContext<PawsomePetsMongoDbContext>(options =>
        {
            options.AddDefaultRepositories();
            options.AddRepository<DogPet, DogPets.MongoDogPetRepository>();

            options.AddRepository<DogPetClient, DogPetsClient.MongoDogPetClientRepository>();

        });

        context.Services.AddAlwaysDisableUnitOfWorkTransaction();
        Configure<AbpUnitOfWorkDefaultOptions>(options =>
        {
            options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
        });
    }
}