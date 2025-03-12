using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PawsomePets.Localization;
using PawsomePets.Permissions;
using PawsomePets.MultiTenancy;
using Volo.Abp.Account.Localization;
using Volo.Abp.UI.Navigation;
using Localization.Resources.AbpUi;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.Users;
using Volo.Abp.Identity.Pro.Blazor.Navigation;
using Volo.Abp.LanguageManagement.Blazor.Menus;
using Volo.Saas.Host.Blazor.Navigation;

namespace PawsomePets.Blazor.Client.Navigation;

public class PawsomePetsMenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public PawsomePetsMenuContributor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }

        else if (context.Menu.Name == StandardMenus.User)
        {
            await ConfigureUserMenuAsync(context);
        }
    }

    private static async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<PawsomePetsResource>();

        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 5;

        context.Menu.AddItem(new ApplicationMenuItem(
            PawsomePetsMenus.Home,
            l["Menu:Home"],
            "/",
            icon: "fas fa-home",
            order: 1
        ));

        //HostDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                PawsomePetsMenus.HostDashboard,
                l["Menu:Dashboard"],
                "/HostDashboard",
                icon: "fa fa-chart-line",
                order: 2
            ).RequirePermissions(PawsomePetsPermissions.Dashboard.Host)
        );

        //TenantDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                PawsomePetsMenus.TenantDashboard,
                l["Menu:Dashboard"],
                "/Dashboard",
                icon: "fa fa-chart-line",
                order: 2
            ).RequirePermissions(PawsomePetsPermissions.Dashboard.Tenant)
        );

        //Saas
        administration.SetSubItemOrder(SaasHostMenus.GroupName, 1);

        //Administration->Identity
        administration.SetSubItemOrder(IdentityProMenus.GroupName, 2);

        //Administration->Language Management
        administration.SetSubItemOrder(LanguageManagementMenus.GroupName, 4);

        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 7);

        context.Menu.AddItem(
            new ApplicationMenuItem(
                PawsomePetsMenus.DogPets,
                l["Menu:DogPets"],
                url: "/dog-pets",
icon: "fa fa-file-alt",
                requiredPermissionName: PawsomePetsPermissions.DogPets.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                PawsomePetsMenus.DogPetsClient,
                l["Menu:DogPetsClient"],
                url: "/dog-pets-client",
                icon: "fa fa-dog",
                requiredPermissionName: PawsomePetsPermissions.DogPetsClient.Default)
        );
    }

    private async Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        if (OperatingSystem.IsBrowser())
        {
            //Blazor wasm menu items

            var authServerUrl = _configuration["AuthServer:Authority"] ?? "";
            var accountResource = context.GetLocalizer<AccountResource>();

            context.Menu.AddItem(new ApplicationMenuItem("Account.Manage", accountResource["MyAccount"], $"{authServerUrl.EnsureEndsWith('/')}Account/Manage", icon: "fa fa-cog", order: 900, target: "_blank").RequireAuthenticated());
            context.Menu.AddItem(new ApplicationMenuItem("Account.SecurityLogs", accountResource["MySecurityLogs"], $"{authServerUrl.EnsureEndsWith('/')}Account/SecurityLogs", icon: "fa fa-user-shield", order: 901, target: "_blank").RequireAuthenticated());
            context.Menu.AddItem(new ApplicationMenuItem("Account.Sessions", accountResource["Sessions"], url: $"{authServerUrl.EnsureEndsWith('/')}Account/Sessions", icon: "fa fa-clock", order: 902, target: "_blank").RequireAuthenticated());

        }
        else
        {
            //Blazor server menu items

        }
        await Task.CompletedTask;
    }
}