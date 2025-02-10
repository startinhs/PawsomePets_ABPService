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
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<PawsomePetsResource>(name);
    }
}