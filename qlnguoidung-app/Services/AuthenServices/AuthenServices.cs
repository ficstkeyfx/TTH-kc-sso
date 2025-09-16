using QLNguoiDung.Services.AuthenServices;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using QLNguoiDung.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.IdentityModel.Tokens.Jwt;
namespace QLNguoiDung
{
    public class AuthenServices:IAuthenServices
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private string token = "";
        private readonly HttpClient _httpClient;
        public AuthenServices(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _httpClient = httpClient;
        }

        public async Task<string> GetCurrentUserTokenAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            token = authState.User.Identity?.Name ?? "";
            return token;
        }
        public string GetCurrentToken()
        {
            var response = GetCurrentUserTokenAsync().GetAwaiter().GetResult();
            //task.Wait();
            return response;
        }
        public async Task<tbUser> GetMe()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var token = authState.User.Identity?.Name ?? "";

            if (string.IsNullOrEmpty(token))
                throw new Exception("Không tìm thấy token.");

            // Giải mã username từ token Keycloak
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var username = jsonToken?.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value ?? "";

            if (string.IsNullOrEmpty(username))
                throw new Exception("Không lấy được username từ token.");

            // Nếu API /api/AuthKeyCloak/user có sẵn thì vẫn có thể gọi để xác thực lại
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("api/AuthKeyCloak/user");
            if (response.IsSuccessStatusCode)
            {
                var apiData = await response.Content.ReadFromJsonAsync<ServiceResponse<tbUser>>();
                if (apiData?.Data != null)
                    return apiData.Data;
            }

            // Nếu không gọi được API thì vẫn trả về tbUser tối thiểu với username
            return new tbUser { UserName = username };
        }
    }
}

