using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace PawsomePets.Data;

/* This is used if database provider does't define
 * IPawsomePetsDbSchemaMigrator implementation.
 */
public class NullPawsomePetsDbSchemaMigrator : IPawsomePetsDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
