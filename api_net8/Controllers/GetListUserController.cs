using api.Services.KeyCloakServices;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace api.SsoKeyCloak.Controllers
{
    //public class UserDto
    //{
    //    public string Username { get; set; }
    //    public string Name { get; set; }
    //}

    [ApiController]
    [Route("api/[controller]")]
    public class GetListUserController : ControllerBase
    {
        private readonly IKeyCloakService _keycloakRepo;
        //private readonly HttpClient _httpClient;
        //private readonly string _realm = "test_client"; // đổi theo realm của bạn
        //private readonly string _baseUrl = "http://192.168.164.145:8080"; // đổi theo server Keycloak

        //public GetListUserController()
        //{
        //    _httpClient = new HttpClient();
        //}

        //private async Task<string?> GetAdminTokenAsync()
        //{
        //    var tokenEndpoint = $"{_baseUrl}/realms/{_realm}/protocol/openid-connect/token";

        //    var parameters = new Dictionary<string, string>
        //    {
        //        { "client_id", "admin-cli" },
        //        { "grant_type", "password" },
        //        { "username", "admin" },
        //        { "password", "admin" }
        //    };

        //    var response = await _httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(parameters));
        //    if (!response.IsSuccessStatusCode) return null;

        //    var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        //    return json.GetProperty("access_token").GetString();
        //}

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users= _keycloakRepo.GetUsers();

            return Ok(users);
        }
    }
}
