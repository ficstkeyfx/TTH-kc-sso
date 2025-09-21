using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using api.Services.KeyCloakServices;
using api.Services.AuthServices;
namespace api.UserSync
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResetPasswordController : ControllerBase
    {
        private readonly IKeyCloakService _keycloakRepo;
        //private readonly HttpClient _httpClient;
        private readonly ILogger<ResetPasswordController> _logger;
        private readonly string _connectionString = "Server=10.0.26.54,1433;Database=keycloak_db;User Id=sa;Password=@Abc12345;TrustServerCertificate=True;";

        //public ResetPasswordController(HttpClient httpClient, ILogger<ResetPasswordController> logger)
        //{
        //    _httpClient = httpClient ?? new HttpClient();
        //    _logger = logger;
        //}

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            _logger.LogInformation($"Processing password reset for user: {request.Username}");

            // 1️⃣ Xác thực thông tin user
            //if (!await ValidateUserInfo(request))
            
            if (!await _keycloakRepo.ValidateUserInfo(request))
            {
                _logger.LogWarning("User information does not match.");
                return BadRequest("Invalid user information.");
            }

            // 3️⃣ Cập nhật mật khẩu trên Keycloak
            bool isPasswordUpdated = await _keycloakRepo.ResetPassword(request.Username, request.NewPassword);
            if (!isPasswordUpdated)
            {
                return StatusCode(500, "Failed to update password in Keycloak.");
            }

            return Ok(new { message = "Password reset successfully" });
        }

        //private async Task<bool> ValidateUserInfo(ResetPasswordRequest request)
        //{
        //    try
        //    {
        //        using var connection = new SqlConnection(_connectionString);
        //        await connection.OpenAsync();

        //        string query = @"
        //            SELECT COUNT(*) FROM users 
        //            WHERE username = @Username 
        //            AND email = @Email 
        //            AND SDT = @SDT 
        //            AND CCCD = @CCCD;";

        //        using var command = new SqlCommand(query, connection);
        //        command.Parameters.AddWithValue("@Username", request.Username);
        //        command.Parameters.AddWithValue("@Email", request.Email);
        //        command.Parameters.AddWithValue("@SDT", request.SDT);
        //        command.Parameters.AddWithValue("@CCCD", request.CCCD);

        //        var count = Convert.ToInt32(await command.ExecuteScalarAsync());
        //        return count > 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Database error: {ex.Message}");
        //        return false;
        //    }
        //}

        //private async Task<bool> UpdatePasswordOnKeycloak(string username, string newPassword)
        //{
        //    try
        //    {
        //        var adminToken = await GetAdminToken();
        //        if (string.IsNullOrEmpty(adminToken))
        //        {
        //            _logger.LogError("Failed to obtain Keycloak admin token.");
        //            return false;
        //        }

        //        // 🔹 Lấy userId từ Keycloak dựa vào username
        //        string keycloakApiUrl = $"http://10.0.26.54:8080/admin/realms/test_client/users?username={username}";
        //        var request = new HttpRequestMessage(HttpMethod.Get, keycloakApiUrl);
        //        request.Headers.Add("Authorization", $"Bearer {adminToken}");

        //        var response = await _httpClient.SendAsync(request);
        //        if (!response.IsSuccessStatusCode)
        //        {
        //            _logger.LogError($"Failed to fetch user ID from Keycloak. Status: {response.StatusCode}");
        //            return false;
        //        }

        //        var content = await response.Content.ReadAsStringAsync();
        //        var users = JsonConvert.DeserializeObject<List<KeycloakUserReset>>(content);
        //        if (users == null || users.Count == 0)
        //        {
        //            _logger.LogError("User not found in Keycloak.");
        //            return false;
        //        }

        //        string userId = users[0].Id;
        //        _logger.LogInformation($"Updating password for Keycloak user ID: {userId}");

        //        // 🔹 Cập nhật mật khẩu mới
        //        string passwordUpdateUrl = $"http://10.0.26.54:8080/admin/realms/test_client/users/{userId}/reset-password";
        //        var passwordUpdatePayload = new
        //        {
        //            type = "password",
        //            value = newPassword,
        //            temporary = false
        //        };

        //        var passwordUpdateRequest = new HttpRequestMessage(HttpMethod.Put, passwordUpdateUrl)
        //        {
        //            Headers = { { "Authorization", $"Bearer {adminToken}" } },
        //            Content = new StringContent(JsonConvert.SerializeObject(passwordUpdatePayload), Encoding.UTF8, "application/json")
        //        };

        //        var passwordUpdateResponse = await _httpClient.SendAsync(passwordUpdateRequest);
        //        if (!passwordUpdateResponse.IsSuccessStatusCode)
        //        {
        //            _logger.LogError($"Failed to update password in Keycloak. Status: {passwordUpdateResponse.StatusCode}");
        //            return false;
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error updating password in Keycloak: {ex.Message}");
        //        return false;
        //    }
        //}

        //private async Task<string> GetAdminToken()
        //{
        //    try
        //    {
        //        var tokenEndpoint = "http://10.0.26.54:8080/realms/test_client/protocol/openid-connect/token";

        //        var parameters = new Dictionary<string, string>
        //        {
        //            { "client_id", "TestSSO" },
        //            { "username", "admin" },
        //            { "password", "admin" },
        //            { "grant_type", "password" },
        //            { "client_secret", "admin-cli-secret" }
        //        };

        //        using var requestContent = new FormUrlEncodedContent(parameters);
        //        using var response = await _httpClient.PostAsync(tokenEndpoint, requestContent);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var tokenResponse = await response.Content.ReadAsStringAsync();
        //            var tokenData = JsonConvert.DeserializeObject<TokenResponse>(tokenResponse);
        //            return tokenData?.Access_token ?? string.Empty;
        //        }

        //        _logger.LogError($"Failed to get Keycloak admin token. Status: {response.StatusCode}");
        //        return string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Exception in GetAdminToken: {ex.Message}");
        //        return string.Empty;
        //    }
        //}
    }

    //public class ResetPasswordRequest
    //{
    //    public string Username { get; set; }
    //    public string Email { get; set; }
    //    public string SDT { get; set; }
    //    public string CCCD { get; set; }
    //    public string NewPassword { get; set; }  // 🔹 Nhận mật khẩu từ client
    //}

    //public class KeycloakUserReset
    //{
    //    public string Id { get; set; }
    //    public string Username { get; set; }
    //}

    //public class TokenResponse
    //{
    //    public string Access_token { get; set; }
    //}
}
