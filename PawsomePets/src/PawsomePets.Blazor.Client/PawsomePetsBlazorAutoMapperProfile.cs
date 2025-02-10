using Volo.Abp.AutoMapper;
using PawsomePets.DogPets;
using AutoMapper;

namespace PawsomePets.Blazor.Client;

public class PawsomePetsBlazorAutoMapperProfile : Profile
{
    public PawsomePetsBlazorAutoMapperProfile()
    {
        //Define your AutoMapper configuration here for the Blazor project.

        CreateMap<DogPetDto, DogPetUpdateDto>();
    }
}