using System;
using PawsomePets.Shared;
using Volo.Abp.AutoMapper;
using PawsomePets.DogPets;
using AutoMapper;

namespace PawsomePets;

public class PawsomePetsApplicationAutoMapperProfile : Profile
{
    public PawsomePetsApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<DogPet, DogPetDto>();
        CreateMap<DogPet, DogPetExcelDto>();

        CreateMap<AppFileDescriptors.AppFileDescriptor, AppFileDescriptorDto>();
    }
}