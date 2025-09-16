using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using api.Services.KeyCloakServices;
namespace api.SsoKeyCloak
{
    //public partial class KeycloakAdminService
    //{
    //    // Xóa user theo userId
    //    public async Task<bool> DeleteUserAsync(string userId)
    //    {
    //        var adminToken = await GetAdminTokenAsync();
    //        if (string.IsNullOrEmpty(adminToken))
    //        {
    //            return false;
    //        }

    //        var deleteUserEndpoint = $"{_baseUrl}/admin/realms/{_realm}/users/{userId}";

    //        _httpClient.DefaultRequestHeaders.Clear();
    //        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {adminToken}");

    //        var response = await _httpClient.DeleteAsync(deleteUserEndpoint);
    //        return response.IsSuccessStatusCode;
    //    }

    //    // Lấy thông tin user theo username
    //    public async Task<KeycloakUserResponse?> GetUserByUsernameAsync(string username)
    //    {
    //        var adminToken = await GetAdminTokenAsync();
    //        if (string.IsNullOrEmpty(adminToken))
    //        {
    //            return null;
    //        }

    //        var searchEndpoint = $"{_baseUrl}/admin/realms/{_realm}/users?username={username}&exact=true";

    //        _httpClient.DefaultRequestHeaders.Clear();
    //        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {adminToken}");

    //        var response = await _httpClient.GetAsync(searchEndpoint);
    //        if (!response.IsSuccessStatusCode)
    //        {
    //            return null;
    //        }

    //        var users = await response.Content.ReadFromJsonAsync<List<KeycloakUserResponse>>();
    //        return users?.FirstOrDefault();
    //    }
    //}

    //public class DeleteUserRequest
    //{
    //    public string Username { get; set; } = string.Empty;
    //    // public string Email { get; set; } = string.Empty;
    //    // public string FirstName { get; set; } = string.Empty;
    //    // public string LastName { get; set; } = string.Empty;
    //}

    [ApiController]
    [Route("api/[controller]")]
    public class DeleteUserController : ControllerBase
    {
        //private readonly KeycloakAdminService _adminService;
        private readonly IKeyCloakService _keycloakRepo;
        //public DeleteUserController()
        //{
        //    _adminService = new KeycloakAdminService();
        //}

        /// <summary>
        /// Xóa user nếu email, firstname, lastname trùng khớp
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request)
        {
            // 1. Lấy thông tin user từ Keycloak
            var user = await _keycloakRepo.GetUserByUsername(request.Username);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            // // 2. Kiểm tra email, firstname, lastname
            // if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase) ||
            //     !string.Equals(user.FirstName, request.FirstName, StringComparison.OrdinalIgnoreCase) ||
            //     !string.Equals(user.LastName, request.LastName, StringComparison.OrdinalIgnoreCase))
            // {
            //     return BadRequest(new { Message = "User information does not match" });
            // }

            // 3. Xóa user
            var success = await _keycloakRepo.DeleteUser(user.Id);
            if (success)
            {
                return Ok(new { Message = "User deleted successfully", UserId = user.Id });
            }

            return BadRequest(new { Message = "Failed to delete user" });
        }
    }
}
