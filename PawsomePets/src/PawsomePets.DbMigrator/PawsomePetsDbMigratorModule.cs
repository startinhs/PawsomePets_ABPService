using PawsomePets.MongoDB;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace PawsomePets.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(PawsomePetsMongoDbModule),
    typeof(PawsomePetsApplicationContractsModule)
)]
public class PawsomePetsDbMigratorModule : AbpModule
{
}
