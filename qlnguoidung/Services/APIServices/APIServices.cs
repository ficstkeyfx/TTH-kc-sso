using QLNguoiDung.Services.IApiServices;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using System.Linq.Expressions;
using System.Net.Http.Headers;
namespace QLNguoiDung
{
    public class ApiServices : IApiServices
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
                  .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                  .AddJsonFile("appsettings.json")
                  .Build();
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        readonly NotificationService _notificationService;
        public ApiServices(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, NotificationService notificationService)
        {
            httpClient = new HttpClient { BaseAddress = new Uri(configuration.GetConnectionString("API") ?? "http://localhost:7239") };
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _notificationService = notificationService;
        }

        // Helper method to retrieve the token
        private async Task<string> GetTokenAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            string token = authState.User.Identity?.Name ?? "";
            return token;
        }
        public async Task<List<T>> GetAll<T>(string apiPath,  string lamda = null)
        {
            string token = await GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response;
            if (lamda == null)
            {
                apiPath = apiPath + "?lamda=\" \"";
            }
            else
            {
                apiPath = apiPath + "?lamda=" + lamda;
            }
            response = await _httpClient.GetAsync(apiPath);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<ServiceResponse<List<T>>>();
                if (data == null || data.Data == null)
                {
                    throw new NullReferenceException("The API response or Data is null.");
                }
                return data.Data;
            }
            else
            {
                if(response.StatusCode==System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {response.ReasonPhrase}");
                }    
                
            }
        }

        public async Task<List<object>> GetByColumns(string apiPath, string columns = "", string lamda = null)
        {
            string token = await GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response;
            if (lamda == null)
            {
                apiPath = apiPath + "?columns=" + columns + "&lamda=\" \"";
            }
            else
            {
                apiPath = apiPath + "?columns=" + columns + "&lamda=" + lamda;
            }
            response = await _httpClient.GetAsync(apiPath);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<ServiceResponse<List<object>>>();
                if (data == null || data.Data == null)
                {
                    throw new NullReferenceException("The API response or Data is null.");
                }
                return data.Data;
            }
            else
            {
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {response.ReasonPhrase}");
            }
        }

        public async Task<T> GetId<T>(string apiPath, string seqName)
        {
            string token = await GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = _httpClient.GetAsync(apiPath + $"?seqName={seqName}").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadFromJsonAsync<ServiceResponse<T>>().Result;
                if (data == null || data.Data == null)
                {
                    throw new NullReferenceException("The API response or Data is null.");
                }
                return data.Data;
            }
            else
            {
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {response.ReasonPhrase}");
            }
        }
 
        public async Task Create<T>(string apiPath, object data)
        {
            string token = await GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var jsonContent = JsonContent.Create(data);
            var response = _httpClient.PostAsync(apiPath, jsonContent).Result;
            if (response.IsSuccessStatusCode)
            {
                _notificationService.Notify(NotificationSeverity.Success, $"Thông báo:", "Đã thêm mới dữ liệu", duration: 1500);
            }
            else
            {
                _notificationService.Notify(NotificationSeverity.Error, $"Có lỗi đã xảy ra", duration: -1);
            }
        }

        public async Task Update<T>(string apiPath, object data)
        {
            string token = await GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var jsonContent = JsonContent.Create(data);
            var response = _httpClient.PutAsync(apiPath, jsonContent).Result;
            if (response.IsSuccessStatusCode)
            {
                _notificationService.Notify(NotificationSeverity.Success, $"Thông báo:", "Đã cập nhật dữ liệu dữ liệu", duration: 1500);
            }
            else
            {
                _notificationService.Notify(NotificationSeverity.Error, $"Có lỗi đã xảy ra", duration: -1);
            }
        }

        /// <summary>
        /// Phương thức xóa các bản ghi
        /// </summary>
        /// <param name="url">Đường dẫn đến API và các tham số</param>
        /// <returns></returns>
        public async Task Delete<T>(string apiPath, string lamda = "", bool showMessage=true)
        {
            string token = await GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            apiPath = apiPath + "?lamda=" + lamda;
            var response =  _httpClient.DeleteAsync(apiPath).Result;
            if (response.IsSuccessStatusCode)
            {
                if(showMessage)
                    _notificationService.Notify(NotificationSeverity.Info, $"Xoá dữ liệu thành công", duration: 2000);
            }
            else
            {
                if (showMessage)
                    _notificationService.Notify(NotificationSeverity.Error, $"Lỗi khi xoá dữ liệu", duration: -1);
            }
        }

        public async Task Delete<T>(string apiPath, List<T> data)
        {
            string token = await GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var jsonContent = JsonContent.Create(data);
            var response = _httpClient.PutAsync(apiPath, jsonContent).Result;
            if (response.IsSuccessStatusCode)
            {
                _notificationService.Notify(NotificationSeverity.Info, $"Xoá dữ liệu thành công", duration: 2000);
            }
            else
            {
                _notificationService.Notify(NotificationSeverity.Error, $"Lỗi khi xoá dữ liệu", duration: -1);
            }
        }
    }
}