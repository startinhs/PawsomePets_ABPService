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
using PawsomePets.DogPetsClient;
using PawsomePets.Permissions;
using PawsomePets.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace PawsomePets.Blazor.Client.Pages
{
    public partial class DogPetsClient
    {
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }
            
        private IJSObjectReference? _jsObjectRef;
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<DogPetClientDto> DogPetClientList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateDogPetClient { get; set; }
        private bool CanEditDogPetClient { get; set; }
        private bool CanDeleteDogPetClient { get; set; }
        private DogPetClientCreateDto NewDogPetClient { get; set; }
        private Validations NewDogPetClientValidations { get; set; } = new();
        private DogPetClientUpdateDto EditingDogPetClient { get; set; }
        private Validations EditingDogPetClientValidations { get; set; } = new();
        private int EditingDogPetClientId { get; set; }
        private Modal CreateDogPetClientModal { get; set; } = new();
        private Modal EditDogPetClientModal { get; set; } = new();
        private GetDogPetsClientInput Filter { get; set; }
        private DataGridEntityActionsColumn<DogPetClientDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "dogPetClient-create-tab";
        protected string SelectedEditTab = "dogPetClient-edit-tab";
        private DogPetClientDto? SelectedDogPetClient;
        
        
        
        
        
        private List<DogPetClientDto> SelectedDogPetsClient { get; set; } = new();
        private bool AllDogPetsClientSelected { get; set; }
        
        public DogPetsClient()
        {
            NewDogPetClient = new DogPetClientCreateDto();
            EditingDogPetClient = new DogPetClientUpdateDto();
            Filter = new GetDogPetsClientInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            DogPetClientList = new List<DogPetClientDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _jsObjectRef = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/Pages/DogPetsClient.razor.js");
                await SetBreadcrumbItemsAsync();
                await SetToolbarItemsAsync();
                await InvokeAsync(StateHasChanged);
            }
        }  

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["DogPetsClient"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewDogPetClient"], async () =>
            {
                await OpenCreateDogPetClientModalAsync();
            }, IconName.Add, requiredPolicyName: PawsomePetsPermissions.DogPetsClient.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateDogPetClient = await AuthorizationService
                .IsGrantedAsync(PawsomePetsPermissions.DogPetsClient.Create);
            CanEditDogPetClient = await AuthorizationService
                            .IsGrantedAsync(PawsomePetsPermissions.DogPetsClient.Edit);
            CanDeleteDogPetClient = await AuthorizationService
                            .IsGrantedAsync(PawsomePetsPermissions.DogPetsClient.Delete);
                            
                            
        }

        private async Task GetDogPetsClientAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await DogPetsClientAppService.GetListAsync(Filter);
            DogPetClientList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetDogPetsClientAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await DogPetsClientAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("PawsomePets") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/dog-pets-client/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}&Breed={HttpUtility.UrlEncode(Filter.Breed)}&AgeMin={Filter.AgeMin}&AgeMax={Filter.AgeMax}&Gender={HttpUtility.UrlEncode(Filter.Gender)}&Color={HttpUtility.UrlEncode(Filter.Color)}&WeightMin={Filter.WeightMin}&WeightMax={Filter.WeightMax}&HealthStatus={HttpUtility.UrlEncode(Filter.HealthStatus)}&VaccinationsMin={Filter.VaccinationsMin}&VaccinationsMax={Filter.VaccinationsMax}&PriceMin={Filter.PriceMin}&PriceMax={Filter.PriceMax}&PromotionPecentsMin={Filter.PromotionPecentsMin}&PromotionPecentsMax={Filter.PromotionPecentsMax}&IsStock={Filter.IsStock}&OtherInformation={HttpUtility.UrlEncode(Filter.OtherInformation)}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<DogPetClientDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetDogPetsClientAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateDogPetClientModalAsync()
        {
            NewDogPetClient = new DogPetClientCreateDto{
                
                
            };

            SelectedCreateTab = "dogPetClient-create-tab";
            
            await _jsObjectRef!.InvokeVoidAsync("FileCleanup.clearInputFiles");
            await NewDogPetClientValidations.ClearAll();
            await CreateDogPetClientModal.Show();
        }

        private async Task CloseCreateDogPetClientModalAsync()
        {
            NewDogPetClient = new DogPetClientCreateDto{
                
                
            };
            await CreateDogPetClientModal.Hide();
        }

        private async Task OpenEditDogPetClientModalAsync(DogPetClientDto input)
        {
            SelectedEditTab = "dogPetClient-edit-tab";
            
            await _jsObjectRef!.InvokeVoidAsync("FileCleanup.clearInputFiles");
            var dogPetClient = await DogPetsClientAppService.GetAsync(input.Id);
            
            EditingDogPetClientId = dogPetClient.Id;
            EditingDogPetClient = ObjectMapper.Map<DogPetClientDto, DogPetClientUpdateDto>(dogPetClient);
            HasSelectedDogPetClientImage = EditingDogPetClient.ImageId != null && EditingDogPetClient.ImageId != Guid.Empty;

            await EditingDogPetClientValidations.ClearAll();
            await EditDogPetClientModal.Show();
        }

        private async Task DeleteDogPetClientAsync(DogPetClientDto input)
        {
            await DogPetsClientAppService.DeleteAsync(input.Id);
            await GetDogPetsClientAsync();
        }

        private async Task CreateDogPetClientAsync()
        {
            try
            {
                if (await NewDogPetClientValidations.ValidateAll() == false)
                {
                    return;
                }

                await DogPetsClientAppService.CreateAsync(NewDogPetClient);
                await GetDogPetsClientAsync();
                await CloseCreateDogPetClientModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditDogPetClientModalAsync()
        {
            await EditDogPetClientModal.Hide();
        }

        private async Task UpdateDogPetClientAsync()
        {
            try
            {
                if (await EditingDogPetClientValidations.ValidateAll() == false)
                {
                    return;
                }

                await DogPetsClientAppService.UpdateAsync(EditingDogPetClientId, EditingDogPetClient);
                await GetDogPetsClientAsync();
                await EditDogPetClientModal.Hide();                
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


        private bool IsCreateFormDisabled()
        {
            return OnNewDogPetClientImageLoading ||NewDogPetClient.ImageId == Guid.Empty ;
        }
        
        private bool IsEditFormDisabled()
        {
            return OnEditDogPetClientImageLoading ||EditingDogPetClient.ImageId == Guid.Empty ;
        }



        private int MaxDogPetClientImageFileUploadSize = 1024 * 1024 * 10; //10MB
        private bool OnNewDogPetClientImageLoading = false;
        private async Task OnNewDogPetClientImageChanged(InputFileChangeEventArgs e)
        {
            try
            {
                if (e.FileCount is 0 or > 1 || e.File.Size > MaxDogPetClientImageFileUploadSize)
                {
                    throw new UserFriendlyException(L["UploadFailedMessage"]);
                }
    
                OnNewDogPetClientImageLoading = true;
                
                var result = await UploadFileAsync(e.File!);
    
                NewDogPetClient.ImageId = result.Id;
                OnNewDogPetClientImageLoading = false;            
            }
            catch(Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        private bool HasSelectedDogPetClientImage = false;
        private bool OnEditDogPetClientImageLoading = false;
        private async Task OnEditDogPetClientImageChanged(InputFileChangeEventArgs e)
        {
            try
            {
                if (e.FileCount is 0 or > 1 || e.File.Size > MaxDogPetClientImageFileUploadSize)
                {
                    throw new UserFriendlyException(L["UploadFailedMessage"]);
                }
    
                OnEditDogPetClientImageLoading = true;
                
                var result = await UploadFileAsync(e.File!);
    
                EditingDogPetClient.ImageId = result.Id;
                OnEditDogPetClientImageLoading = false;            
            }
            catch(Exception ex)
            {
                await HandleErrorAsync(ex);
            }            
        }




        private async Task<AppFileDescriptorDto> UploadFileAsync(IBrowserFile file)
        {
            using (var ms = new MemoryStream())
            {
                await file.OpenReadStream(long.MaxValue).CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                
                return await DogPetsClientAppService.UploadFileAsync(new RemoteStreamContent(ms, file.Name, file.ContentType));
            }
        }



        private async Task DownloadFileAsync(Guid fileId)
        {
            var token = (await DogPetsClientAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("PawsomePets") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/dog-pets-client/file?DownloadToken={token}&FileId={fileId}", forceLoad: true);
        }

        protected virtual async Task OnNameChangedAsync(string? name)
        {
            Filter.Name = name;
            await SearchAsync();
        }
        protected virtual async Task OnBreedChangedAsync(string? breed)
        {
            Filter.Breed = breed;
            await SearchAsync();
        }
        protected virtual async Task OnAgeMinChangedAsync(float? ageMin)
        {
            Filter.AgeMin = ageMin;
            await SearchAsync();
        }
        protected virtual async Task OnAgeMaxChangedAsync(float? ageMax)
        {
            Filter.AgeMax = ageMax;
            await SearchAsync();
        }
        protected virtual async Task OnGenderChangedAsync(string? gender)
        {
            Filter.Gender = gender;
            await SearchAsync();
        }
        protected virtual async Task OnColorChangedAsync(string? color)
        {
            Filter.Color = color;
            await SearchAsync();
        }
        protected virtual async Task OnWeightMinChangedAsync(float? weightMin)
        {
            Filter.WeightMin = weightMin;
            await SearchAsync();
        }
        protected virtual async Task OnWeightMaxChangedAsync(float? weightMax)
        {
            Filter.WeightMax = weightMax;
            await SearchAsync();
        }
        protected virtual async Task OnHealthStatusChangedAsync(string? healthStatus)
        {
            Filter.HealthStatus = healthStatus;
            await SearchAsync();
        }
        protected virtual async Task OnVaccinationsMinChangedAsync(int? vaccinationsMin)
        {
            Filter.VaccinationsMin = vaccinationsMin;
            await SearchAsync();
        }
        protected virtual async Task OnVaccinationsMaxChangedAsync(int? vaccinationsMax)
        {
            Filter.VaccinationsMax = vaccinationsMax;
            await SearchAsync();
        }
        protected virtual async Task OnPriceMinChangedAsync(decimal? priceMin)
        {
            Filter.PriceMin = priceMin;
            await SearchAsync();
        }
        protected virtual async Task OnPriceMaxChangedAsync(decimal? priceMax)
        {
            Filter.PriceMax = priceMax;
            await SearchAsync();
        }
        protected virtual async Task OnPromotionPecentsMinChangedAsync(float? promotionPecentsMin)
        {
            Filter.PromotionPecentsMin = promotionPecentsMin;
            await SearchAsync();
        }
        protected virtual async Task OnPromotionPecentsMaxChangedAsync(float? promotionPecentsMax)
        {
            Filter.PromotionPecentsMax = promotionPecentsMax;
            await SearchAsync();
        }
        protected virtual async Task OnIsStockChangedAsync(bool? isStock)
        {
            Filter.IsStock = isStock;
            await SearchAsync();
        }
        protected virtual async Task OnOtherInformationChangedAsync(string? otherInformation)
        {
            Filter.OtherInformation = otherInformation;
            await SearchAsync();
        }
        





        private Task SelectAllItems()
        {
            AllDogPetsClientSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllDogPetsClientSelected = false;
            SelectedDogPetsClient.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedDogPetClientRowsChanged()
        {
            if (SelectedDogPetsClient.Count != PageSize)
            {
                AllDogPetsClientSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedDogPetsClientAsync()
        {
            var message = AllDogPetsClientSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedDogPetsClient.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllDogPetsClientSelected)
            {
                await DogPetsClientAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await DogPetsClientAppService.DeleteByIdsAsync(SelectedDogPetsClient.Select(x => x.Id).ToList());
            }

            SelectedDogPetsClient.Clear();
            AllDogPetsClientSelected = false;

            await GetDogPetsClientAsync();
        }


    }
}
