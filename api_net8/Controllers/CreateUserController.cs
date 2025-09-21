using System.Text;
using System.Text.Json;
using api.Services.KeyCloakServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.SsoKeyCloak
{
    //public partial class KeycloakAdminService
    //{
        //private readonly HttpClient _httpClient;
        //private readonly string _baseUrl = "http://10.0.26.54:8080";
        //private readonly string _realm = "test_client";
        //private readonly string _adminUsername = "admin"; // Replace with your admin username
        //private readonly string _adminPassword = "admin"; // Replace with your admin password
        
        //public KeycloakAdminService()
        //{
        //    _httpClient = new HttpClient();
        //}

    //    private async Task<string?> GetAdminTokenAsync()
    //    {
    //        var tokenEndpoint = $"{_baseUrl}/realms/master/protocol/openid-connect/token";

    //        var parameters = new Dictionary<string, string>
    //        {
    //            { "client_id", "admin-cli" },
    //            { "grant_type", "password" },
    //            { "username", _adminUsername },
    //            { "password", _adminPassword }
    //        };

    //        var response = await _httpClient.PostAsync(tokenEndpoint,
    //            new FormUrlEncodedContent(parameters));

    //        if (response.IsSuccessStatusCode)
    //        {
    //            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
    //            return tokenResponse?.Access_token;
    //        }

    //        return null;
    //    }

    //    public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest userRequest)
    //    {
    //        var adminToken = await GetAdminTokenAsync();
    //        if (string.IsNullOrEmpty(adminToken))
    //        {
    //            return new CreateUserResponse 
    //            { 
    //                Success = false, 
    //                Message = "Failed to obtain admin token" 
    //            };
    //        }

    //        var createUserEndpoint = $"{_baseUrl}/admin/realms/{_realm}/users";

    //        var keycloakUser = new KeycloakUserModel
    //        {
    //            Username = userRequest.Username,
    //            Email = userRequest.Email,
    //            FirstName = userRequest.FirstName,
    //            LastName = userRequest.LastName,
    //            Enabled = true,
    //            EmailVerified = false,
    //            Credentials = new List<KeycloakCredential>
    //            {
    //                new KeycloakCredential
    //                {
    //                    Type = "password",
    //                    Value = userRequest.Password,
    //                    Temporary = false
    //                }
    //            }
    //        };

    //        var json = JsonSerializer.Serialize(keycloakUser, new JsonSerializerOptions
    //        {
    //            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    //        });

    //        _httpClient.DefaultRequestHeaders.Clear();
    //        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {adminToken}");

    //        var content = new StringContent(json, Encoding.UTF8, "application/json");
    //        var response = await _httpClient.PostAsync(createUserEndpoint, content);

    //        if (response.IsSuccessStatusCode)
    //        {
    //            // Get the user ID from the Location header
    //            var locationHeader = response.Headers.Location?.ToString();
    //            var userId = locationHeader?.Split('/').LastOrDefault();

    //            return new CreateUserResponse 
    //            { 
    //                Success = true, 
    //                Message = "User created successfully",
    //                UserId = userId
    //            };
    //        }
    //        else
    //        {
    //            var errorContent = await response.Content.ReadAsStringAsync();
    //            return new CreateUserResponse 
    //            { 
    //                Success = false, 
    //                Message = $"Failed to create user: {response.StatusCode} - {errorContent}" 
    //            };
    //        }
    //    }

    //    public async Task<bool> CheckUserExistsAsync(string username)
    //    {
    //        var adminToken = await GetAdminTokenAsync();
    //        if (string.IsNullOrEmpty(adminToken))
    //        {
    //            return false;
    //        }

    //        var searchEndpoint = $"{_baseUrl}/admin/realms/{_realm}/users?username={username}&exact=true";

    //        _httpClient.DefaultRequestHeaders.Clear();
    //        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {adminToken}");

    //        var response = await _httpClient.GetAsync(searchEndpoint);

    //        if (response.IsSuccessStatusCode)
    //        {
    //            var users = await response.Content.ReadFromJsonAsync<List<KeycloakUserResponse>>();
    //            return users?.Any() == true;
    //        }

    //        return false;
    //    }
    //}

    [ApiController]
    [Route("api/[controller]")]
    public class CreateUserController : ControllerBase
    {
        //private readonly KeycloakAdminService _adminService;
            private readonly IKeyCloakService _keycloakRepo;
        //public CreateUserController()
        //{
        //    _adminService = new KeycloakAdminService();
        //}

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if user already exists
            var userExists = await _keycloakRepo.CheckUserExists(request.Username);
            if (userExists)
            {
                return Conflict(new { Message = "User with this username already exists" });
            }

            var result = await _keycloakRepo.CreateUser(request);

            if (result.Success)
            {
                return CreatedAtAction(nameof(CreateUser), new { id = result.UserId }, result);
            }

            return BadRequest(result);
        }

        [HttpGet("check/{username}")]
        public async Task<IActionResult> CheckUserExists(string username)
        {
            var exists = await _keycloakRepo.CheckUserExists(username);
            return Ok(new { Username = username, Exists = exists });
        }
    }

    // Request/Response Models
    //public class CreateUserRequest
    //{
    //    public string Username { get; set; } = string.Empty;
    //    public string Email { get; set; } = string.Empty;
    //    public string FirstName { get; set; } = string.Empty;
    //    public string LastName { get; set; } = string.Empty;
    //    public string Password { get; set; } = string.Empty;
    //}

    //public class CreateUserResponse
    //{
    //    public bool Success { get; set; }
    //    public string Message { get; set; } = string.Empty;
    //    public string? UserId { get; set; }
    //}

    // Keycloak API Models
    //public class KeycloakUserModel
    //{
    //    public string Username { get; set; } = string.Empty;
    //    public string Email { get; set; } = string.Empty;
    //    public string FirstName { get; set; } = string.Empty;
    //    public string LastName { get; set; } = string.Empty;
    //    public bool Enabled { get; set; } = true;
    //    public bool EmailVerified { get; set; } = false;
    //    public List<KeycloakCredential> Credentials { get; set; } = new();
    //}

    //public class KeycloakCredential
    //{
    //    public string Type { get; set; } = string.Empty;
    //    public string Value { get; set; } = string.Empty;
    //    public bool Temporary { get; set; } = false;
    //}

    //public class KeycloakUserResponse
    //{
    //    public string Id { get; set; } = string.Empty;
    //    public string Username { get; set; } = string.Empty;
    //    public string Email { get; set; } = string.Empty;
    //    public string FirstName { get; set; } = string.Empty;
    //    public string LastName { get; set; } = string.Empty;
    //    public bool Enabled { get; set; }
    //    public bool EmailVerified { get; set; }
    //}
}