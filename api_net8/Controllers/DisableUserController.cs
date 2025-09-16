using api.Services.KeyCloakServices;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using api.Services.KeyCloakServices;
namespace api.SsoKeyCloak
{
    //public partial class KeycloakAdminService
    //{
    //    // Hàm disable user
    //    public async Task<bool> DisableUserAsync(string userId)
    //    {
    //        var adminToken = await GetAdminTokenAsync();
    //        if (string.IsNullOrEmpty(adminToken))
    //        {
    //            return false;
    //        }

    //        var updateUserEndpoint = $"{_baseUrl}/admin/realms/{_realm}/users/{userId}";

    //        _httpClient.DefaultRequestHeaders.Clear();
    //        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {adminToken}");

    //        // Chỉ gửi trường Enabled = false
    //        var payload = new { enabled = false };
    //        var json = JsonSerializer.Serialize(payload);
    //        var content = new StringContent(json, Encoding.UTF8, "application/json");

    //        var response = await _httpClient.PutAsync(updateUserEndpoint, content);
    //        return response.IsSuccessStatusCode;
    //    }
    //}

    //public class DisableUserRequest
    //{
    //    public string Username { get; set; } = string.Empty;
    //    public string Email { get; set; } = string.Empty;
    //    public string FirstName { get; set; } = string.Empty;
    //    public string LastName { get; set; } = string.Empty;
    //}

    [ApiController]
    [Route("api/[controller]")]
    public class DisableUserController : ControllerBase
    {
        //private readonly KeycloakAdminService _adminService;
        private readonly IKeyCloakService _keycloakRepo;
        //public DisableUserController()
        //{
        //    _adminService = new KeycloakAdminService();
        //}

        /// <summary>
        /// Disable user nếu email, firstname, lastname trùng khớp
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> DisableUser([FromBody] DisableUserRequest request)
        {
            // 1. Lấy thông tin user từ Keycloak
            var user = await _keycloakRepo.GetUserByUsername(request.Username);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            // 2. Kiểm tra email, firstname, lastname
            if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(user.FirstName, request.FirstName, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(user.LastName, request.LastName, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { Message = "User information does not match" });
            }

            // 3. Disable user
            var success = await _keycloakRepo.DisableUser(user.Id);
            if (success)
            {
                return Ok(new { Message = "User disabled successfully", UserId = user.Id });
            }

            return BadRequest(new { Message = "Failed to disable user" });
        }
    }
}
