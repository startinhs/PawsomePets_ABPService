using System.Threading.Tasks;

namespace PawsomePets.Data;

public interface IPawsomePetsDbSchemaMigrator
{
    Task MigrateAsync();
}
