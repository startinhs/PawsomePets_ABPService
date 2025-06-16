using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Web;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using PawsomePets.MediaStorages;
using PawsomePets.Permissions;
using PawsomePets.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace PawsomePets.Blazor.Client.Pages
{
    public partial class MediaStorages
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<MediaStorageDto> MediaStorageList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateMediaStorage { get; set; }
        private bool CanEditMediaStorage { get; set; }
        private bool CanDeleteMediaStorage { get; set; }
        private MediaStorageCreateDto NewMediaStorage { get; set; }
        private Validations NewMediaStorageValidations { get; set; } = new();
        private MediaStorageUpdateDto EditingMediaStorage { get; set; }
        private Validations EditingMediaStorageValidations { get; set; } = new();
        private int EditingMediaStorageId { get; set; }
        private Modal CreateMediaStorageModal { get; set; } = new();
        private Modal EditMediaStorageModal { get; set; } = new();
        private GetMediaStoragesInput Filter { get; set; }
        private DataGridEntityActionsColumn<MediaStorageDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "mediaStorage-create-tab";
        protected string SelectedEditTab = "mediaStorage-edit-tab";
        private MediaStorageDto? SelectedMediaStorage;
        
        
        
        
        
        private List<MediaStorageDto> SelectedMediaStorages { get; set; } = new();
        private bool AllMediaStoragesSelected { get; set; }
        
        public MediaStorages()
        {
            NewMediaStorage = new MediaStorageCreateDto();
            EditingMediaStorage = new MediaStorageUpdateDto();
            Filter = new GetMediaStoragesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            MediaStorageList = new List<MediaStorageDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                
                await SetBreadcrumbItemsAsync();
                await SetToolbarItemsAsync();
                await InvokeAsync(StateHasChanged);
            }
        }  

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["MediaStorages"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewMediaStorage"], async () =>
            {
                await OpenCreateMediaStorageModalAsync();
            }, IconName.Add, requiredPolicyName: PawsomePetsPermissions.MediaStorages.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateMediaStorage = await AuthorizationService
                .IsGrantedAsync(PawsomePetsPermissions.MediaStorages.Create);
            CanEditMediaStorage = await AuthorizationService
                            .IsGrantedAsync(PawsomePetsPermissions.MediaStorages.Edit);
            CanDeleteMediaStorage = await AuthorizationService
                            .IsGrantedAsync(PawsomePetsPermissions.MediaStorages.Delete);
                            
                            
        }

        private async Task GetMediaStoragesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await MediaStoragesAppService.GetListAsync(Filter);
            MediaStorageList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetMediaStoragesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await MediaStoragesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("PawsomePets") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/media-storages/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&FileName={HttpUtility.UrlEncode(Filter.FileName)}&FileUrl={HttpUtility.UrlEncode(Filter.FileUrl)}&Description={HttpUtility.UrlEncode(Filter.Description)}&FileType={HttpUtility.UrlEncode(Filter.FileType)}&FileSizeMin={Filter.FileSizeMin}&FileSizeMax={Filter.FileSizeMax}&IsMain={Filter.IsMain}&ProviderName={HttpUtility.UrlEncode(Filter.ProviderName)}&ContainerName={HttpUtility.UrlEncode(Filter.ContainerName)}&EntityIdMin={Filter.EntityIdMin}&EntityIdMax={Filter.EntityIdMax}&EntityType={HttpUtility.UrlEncode(Filter.EntityType)}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<MediaStorageDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetMediaStoragesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateMediaStorageModalAsync()
        {
            NewMediaStorage = new MediaStorageCreateDto{
                
                
            };

            SelectedCreateTab = "mediaStorage-create-tab";
            
            
            await NewMediaStorageValidations.ClearAll();
            await CreateMediaStorageModal.Show();
        }

        private async Task CloseCreateMediaStorageModalAsync()
        {
            NewMediaStorage = new MediaStorageCreateDto{
                
                
            };
            await CreateMediaStorageModal.Hide();
        }

        private async Task OpenEditMediaStorageModalAsync(MediaStorageDto input)
        {
            SelectedEditTab = "mediaStorage-edit-tab";
            
            
            var mediaStorage = await MediaStoragesAppService.GetAsync(input.Id);
            
            EditingMediaStorageId = mediaStorage.Id;
            EditingMediaStorage = ObjectMapper.Map<MediaStorageDto, MediaStorageUpdateDto>(mediaStorage);
            
            await EditingMediaStorageValidations.ClearAll();
            await EditMediaStorageModal.Show();
        }

        private async Task DeleteMediaStorageAsync(MediaStorageDto input)
        {
            await MediaStoragesAppService.DeleteAsync(input.Id);
            await GetMediaStoragesAsync();
        }

        private async Task CreateMediaStorageAsync()
        {
            try
            {
                if (await NewMediaStorageValidations.ValidateAll() == false)
                {
                    return;
                }

                await MediaStoragesAppService.CreateAsync(NewMediaStorage);
                await GetMediaStoragesAsync();
                await CloseCreateMediaStorageModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditMediaStorageModalAsync()
        {
            await EditMediaStorageModal.Hide();
        }

        private async Task UpdateMediaStorageAsync()
        {
            try
            {
                if (await EditingMediaStorageValidations.ValidateAll() == false)
                {
                    return;
                }

                await MediaStoragesAppService.UpdateAsync(EditingMediaStorageId, EditingMediaStorage);
                await GetMediaStoragesAsync();
                await EditMediaStorageModal.Hide();                
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private void OnSelectedCreateTabChanged(string name)
        {
            SelectedCreateTab = name;
        }

        private void OnSelectedEditTabChanged(string name)
        {
            SelectedEditTab = name;
        }









        protected virtual async Task OnFileNameChangedAsync(string? fileName)
        {
            Filter.FileName = fileName;
            await SearchAsync();
        }
        protected virtual async Task OnFileUrlChangedAsync(string? fileUrl)
        {
            Filter.FileUrl = fileUrl;
            await SearchAsync();
        }
        protected virtual async Task OnDescriptionChangedAsync(string? description)
        {
            Filter.Description = description;
            await SearchAsync();
        }
        protected virtual async Task OnFileTypeChangedAsync(string? fileType)
        {
            Filter.FileType = fileType;
            await SearchAsync();
        }
        protected virtual async Task OnFileSizeMinChangedAsync(float? fileSizeMin)
        {
            Filter.FileSizeMin = fileSizeMin;
            await SearchAsync();
        }
        protected virtual async Task OnFileSizeMaxChangedAsync(float? fileSizeMax)
        {
            Filter.FileSizeMax = fileSizeMax;
            await SearchAsync();
        }
        protected virtual async Task OnIsMainChangedAsync(bool? isMain)
        {
            Filter.IsMain = isMain;
            await SearchAsync();
        }
        protected virtual async Task OnProviderNameChangedAsync(string? providerName)
        {
            Filter.ProviderName = providerName;
            await SearchAsync();
        }
        protected virtual async Task OnContainerNameChangedAsync(string? containerName)
        {
            Filter.ContainerName = containerName;
            await SearchAsync();
        }
        protected virtual async Task OnEntityIdMinChangedAsync(int? entityIdMin)
        {
            Filter.EntityIdMin = entityIdMin;
            await SearchAsync();
        }
        protected virtual async Task OnEntityIdMaxChangedAsync(int? entityIdMax)
        {
            Filter.EntityIdMax = entityIdMax;
            await SearchAsync();
        }
        protected virtual async Task OnEntityTypeChangedAsync(string? entityType)
        {
            Filter.EntityType = entityType;
            await SearchAsync();
        }
        





        private Task SelectAllItems()
        {
            AllMediaStoragesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllMediaStoragesSelected = false;
            SelectedMediaStorages.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedMediaStorageRowsChanged()
        {
            if (SelectedMediaStorages.Count != PageSize)
            {
                AllMediaStoragesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedMediaStoragesAsync()
        {
            var message = AllMediaStoragesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedMediaStorages.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllMediaStoragesSelected)
            {
                await MediaStoragesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await MediaStoragesAppService.DeleteByIdsAsync(SelectedMediaStorages.Select(x => x.Id).ToList());
            }

            SelectedMediaStorages.Clear();
            AllMediaStoragesSelected = false;

            await GetMediaStoragesAsync();
        }


    }
}
