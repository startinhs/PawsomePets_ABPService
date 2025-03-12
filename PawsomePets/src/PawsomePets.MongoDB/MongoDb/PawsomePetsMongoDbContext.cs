using PawsomePets.DogPetsClient;
using PawsomePets.DogPets;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;
using MongoDB.Driver;

namespace PawsomePets.MongoDB;

[ConnectionStringName("Default")]
public class PawsomePetsMongoDbContext : AbpMongoDbContext
{
    public IMongoCollection<DogPetClient> DogPetsClient => Collection<DogPetClient>();
    public IMongoCollection<AppFileDescriptors.AppFileDescriptor> AppFileDescriptors => Collection<AppFileDescriptors.AppFileDescriptor>();
    public IMongoCollection<DogPet> DogPets => Collection<DogPet>();

    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        //builder.Entity<YourEntity>(b =>
        //{
        //    //...
        //});

        modelBuilder.Entity<DogPet>(b => { b.CollectionName = PawsomePetsConsts.DbTablePrefix + "DogPets"; });

        modelBuilder.Entity<AppFileDescriptors.AppFileDescriptor>(b => { b.CollectionName = PawsomePetsConsts.DbTablePrefix + "AppFileDescriptors"; });

        modelBuilder.Entity<DogPetClient>(b => { b.CollectionName = PawsomePetsConsts.DbTablePrefix + "DogPets"; });
    }
}