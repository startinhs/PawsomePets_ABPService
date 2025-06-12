using PawsomePets.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace PawsomePets.Permissions;

public class PawsomePetsPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(PawsomePetsPermissions.GroupName);

        myGroup.AddPermission(PawsomePetsPermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
        myGroup.AddPermission(PawsomePetsPermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(PawsomePetsPermissions.MyPermission1, L("Permission:MyPermission1"));

        var dogPetPermission = myGroup.AddPermission(PawsomePetsPermissions.DogPets.Default, L("Permission:DogPets"));
        dogPetPermission.AddChild(PawsomePetsPermissions.DogPets.Create, L("Permission:Create"));
        dogPetPermission.AddChild(PawsomePetsPermissions.DogPets.Edit, L("Permission:Edit"));
        dogPetPermission.AddChild(PawsomePetsPermissions.DogPets.Delete, L("Permission:Delete"));

        var dogPetClientPermission = myGroup.AddPermission(PawsomePetsPermissions.DogPetsClient.Default, L("Permission:DogPetsClient"));
        dogPetClientPermission.AddChild(PawsomePetsPermissions.DogPetsClient.Create, L("Permission:Create"));
        dogPetClientPermission.AddChild(PawsomePetsPermissions.DogPetsClient.Edit, L("Permission:Edit"));
        dogPetClientPermission.AddChild(PawsomePetsPermissions.DogPetsClient.Delete, L("Permission:Delete"));

        var mediaStoragePermission = myGroup.AddPermission(PawsomePetsPermissions.MediaStorages.Default, L("Permission:MediaStorages"));
        mediaStoragePermission.AddChild(PawsomePetsPermissions.MediaStorages.Create, L("Permission:Create"));
        mediaStoragePermission.AddChild(PawsomePetsPermissions.MediaStorages.Edit, L("Permission:Edit"));
        mediaStoragePermission.AddChild(PawsomePetsPermissions.MediaStorages.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<PawsomePetsResource>(name);
    }
}