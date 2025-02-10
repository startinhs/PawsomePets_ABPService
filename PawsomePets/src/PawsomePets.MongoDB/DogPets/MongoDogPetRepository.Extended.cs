using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using PawsomePets.MongoDB;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace PawsomePets.DogPets
{
    public class MongoDogPetRepository : MongoDogPetRepositoryBase, IDogPetRepository
    {
        public MongoDogPetRepository(IMongoDbContextProvider<PawsomePetsMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        //Write your custom code...
    }
}