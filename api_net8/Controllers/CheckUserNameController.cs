using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using api.Services.KeyCloakServices;

namespace api.UserSync
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly string _connectionString = "Server=192.168.164.145,1433;Database=keycloak_db;User Id=sa;Password=@Abc12345;TrustServerCertificate=True;";

        private readonly IKeyCloakService _keycloakRepo;
        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        [HttpGet("CheckUsername")]
        public async Task<IActionResult> CheckUsername([FromQuery] string username)
        {
            _logger.LogInformation($"Checking if username exists: {username}");

            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Username cannot be empty");
            }

            try
            {
                bool exists = await _keycloakRepo.UsernameExistsInDatabase(username);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking username: {ex.Message}");
                return StatusCode(500, "An error occurred while checking the username");
            }
        }

        //private async Task<bool> UsernameExistsInDatabase(string username)
        //{
        //    try
        //    {
        //        using var connection = new SqlConnection(_connectionString);
        //        await connection.OpenAsync();

        //        string query = "SELECT COUNT(*) FROM users WHERE username = @Username";

        //        using var command = new SqlCommand(query, connection);
        //        command.Parameters.AddWithValue("@Username", username);

        //        var count = Convert.ToInt32(await command.ExecuteScalarAsync());
        //        return count > 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Database error when checking username: {ex.Message}");
        //        throw;
        //    }
        //}
    }

    //public class UsernameModel
    //{
    //    public string Username { get; set; }
    //}
}