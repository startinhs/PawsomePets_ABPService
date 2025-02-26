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
using PawsomePets.DogPets;
using PawsomePets.Permissions;
using PawsomePets.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace PawsomePets.Blazor.Client.Pages
{
    public partial class DogPets
    {
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }
            
        private IJSObjectReference? _jsObjectRef;
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<DogPetDto> DogPetList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateDogPet { get; set; }
        private bool CanEditDogPet { get; set; }
        private bool CanDeleteDogPet { get; set; }
        private DogPetCreateDto NewDogPet { get; set; }
        private Validations NewDogPetValidations { get; set; } = new();
        private DogPetUpdateDto EditingDogPet { get; set; }
        private Validations EditingDogPetValidations { get; set; } = new();
        private int EditingDogPetId { get; set; }
        private Modal CreateDogPetModal { get; set; } = new();
        private Modal EditDogPetModal { get; set; } = new();
        private GetDogPetsInput Filter { get; set; }
        private DataGridEntityActionsColumn<DogPetDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "dogPet-create-tab";
        protected string SelectedEditTab = "dogPet-edit-tab";
        private DogPetDto? SelectedDogPet;
        
        
        
        
        
        private List<DogPetDto> SelectedDogPets { get; set; } = new();
        private bool AllDogPetsSelected { get; set; }
        
        public DogPets()
        {
            NewDogPet = new DogPetCreateDto();
            EditingDogPet = new DogPetUpdateDto();
            Filter = new GetDogPetsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            DogPetList = new List<DogPetDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _jsObjectRef = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/Pages/DogPets.razor.js");
                await SetBreadcrumbItemsAsync();
                await SetToolbarItemsAsync();
                await InvokeAsync(StateHasChanged);
            }
        }  

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["DogPets"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewDogPet"], async () =>
            {
                await OpenCreateDogPetModalAsync();
            }, IconName.Add, requiredPolicyName: PawsomePetsPermissions.DogPets.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateDogPet = await AuthorizationService
                .IsGrantedAsync(PawsomePetsPermissions.DogPets.Create);
            CanEditDogPet = await AuthorizationService
                            .IsGrantedAsync(PawsomePetsPermissions.DogPets.Edit);
            CanDeleteDogPet = await AuthorizationService
                            .IsGrantedAsync(PawsomePetsPermissions.DogPets.Delete);
                            
                            
        }

        private async Task GetDogPetsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await DogPetsAppService.GetListAsync(Filter);
            DogPetList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetDogPetsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await DogPetsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("PawsomePets") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/dog-pets/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}&Breed={HttpUtility.UrlEncode(Filter.Breed)}&AgeMin={Filter.AgeMin}&AgeMax={Filter.AgeMax}&Gender={HttpUtility.UrlEncode(Filter.Gender)}&Color={HttpUtility.UrlEncode(Filter.Color)}&WeightMin={Filter.WeightMin}&WeightMax={Filter.WeightMax}&HealthStatus={HttpUtility.UrlEncode(Filter.HealthStatus)}&VaccinationsMin={Filter.VaccinationsMin}&VaccinationsMax={Filter.VaccinationsMax}&PriceMin={Filter.PriceMin}&PriceMax={Filter.PriceMax}&PromotionPecentsMin={Filter.PromotionPecentsMin}&PromotionPecentsMax={Filter.PromotionPecentsMax}&IsStock={Filter.IsStock}&OtherInformation={HttpUtility.UrlEncode(Filter.OtherInformation)}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<DogPetDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetDogPetsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateDogPetModalAsync()
        {
            NewDogPet = new DogPetCreateDto{
                
                
            };

            SelectedCreateTab = "dogPet-create-tab";
            
            await _jsObjectRef!.InvokeVoidAsync("FileCleanup.clearInputFiles");
            await NewDogPetValidations.ClearAll();
            await CreateDogPetModal.Show();
        }

        private async Task CloseCreateDogPetModalAsync()
        {
            NewDogPet = new DogPetCreateDto{
                
                
            };
            await CreateDogPetModal.Hide();
        }

        private async Task OpenEditDogPetModalAsync(DogPetDto input)
        {
            SelectedEditTab = "dogPet-edit-tab";
            
            await _jsObjectRef!.InvokeVoidAsync("FileCleanup.clearInputFiles");
            var dogPet = await DogPetsAppService.GetAsync(input.Id);
            
            EditingDogPetId = dogPet.Id;
            EditingDogPet = ObjectMapper.Map<DogPetDto, DogPetUpdateDto>(dogPet);
            HasSelectedDogPetImage = EditingDogPet.ImageId != null && EditingDogPet.ImageId != Guid.Empty;

            await EditingDogPetValidations.ClearAll();
            await EditDogPetModal.Show();
        }

        private async Task DeleteDogPetAsync(DogPetDto input)
        {
            await DogPetsAppService.DeleteAsync(input.Id);
            await GetDogPetsAsync();
        }

        private async Task CreateDogPetAsync()
        {
            try
            {
                if (await NewDogPetValidations.ValidateAll() == false)
                {
                    return;
                }

                await DogPetsAppService.CreateAsync(NewDogPet);
                await GetDogPetsAsync();
                await CloseCreateDogPetModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditDogPetModalAsync()
        {
            await EditDogPetModal.Hide();
        }

        private async Task UpdateDogPetAsync()
        {
            try
            {
                if (await EditingDogPetValidations.ValidateAll() == false)
                {
                    return;
                }

                await DogPetsAppService.UpdateAsync(EditingDogPetId, EditingDogPet);
                await GetDogPetsAsync();
                await EditDogPetModal.Hide();                
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
            return OnNewDogPetImageLoading ||NewDogPet.ImageId == Guid.Empty ;
        }
        
        private bool IsEditFormDisabled()
        {
            return OnEditDogPetImageLoading ||EditingDogPet.ImageId == Guid.Empty ;
        }



        private int MaxDogPetImageFileUploadSize = 1024 * 1024 * 10; //10MB
        private bool OnNewDogPetImageLoading = false;
        private async Task OnNewDogPetImageChanged(InputFileChangeEventArgs e)
        {
            try
            {
                if (e.FileCount is 0 or > 1 || e.File.Size > MaxDogPetImageFileUploadSize)
                {
                    throw new UserFriendlyException(L["UploadFailedMessage"]);
                }
    
                OnNewDogPetImageLoading = true;
                
                var result = await UploadFileAsync(e.File!);
    
                NewDogPet.ImageId = result.Id;
                OnNewDogPetImageLoading = false;            
            }
            catch(Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        private bool HasSelectedDogPetImage = false;
        private bool OnEditDogPetImageLoading = false;
        private async Task OnEditDogPetImageChanged(InputFileChangeEventArgs e)
        {
            try
            {
                if (e.FileCount is 0 or > 1 || e.File.Size > MaxDogPetImageFileUploadSize)
                {
                    throw new UserFriendlyException(L["UploadFailedMessage"]);
                }
    
                OnEditDogPetImageLoading = true;
                
                var result = await UploadFileAsync(e.File!);
    
                EditingDogPet.ImageId = result.Id;
                OnEditDogPetImageLoading = false;            
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
                
                return await DogPetsAppService.UploadFileAsync(new RemoteStreamContent(ms, file.Name, file.ContentType));
            }
        }



        private async Task DownloadFileAsync(Guid fileId)
        {
            var token = (await DogPetsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("PawsomePets") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/dog-pets/file?DownloadToken={token}&FileId={fileId}", forceLoad: true);
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
            AllDogPetsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllDogPetsSelected = false;
            SelectedDogPets.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedDogPetRowsChanged()
        {
            if (SelectedDogPets.Count != PageSize)
            {
                AllDogPetsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedDogPetsAsync()
        {
            var message = AllDogPetsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedDogPets.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllDogPetsSelected)
            {
                await DogPetsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await DogPetsAppService.DeleteByIdsAsync(SelectedDogPets.Select(x => x.Id).ToList());
            }

            SelectedDogPets.Clear();
            AllDogPetsSelected = false;

            await GetDogPetsAsync();
        }


    }
}
