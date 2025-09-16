using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace api.UserSync
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserUpdateController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserUpdateController> _logger;

        public UserUpdateController(HttpClient httpClient, ILogger<UserUpdateController> logger)
        {
            _httpClient = httpClient ?? new HttpClient();
            _logger = logger;
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncUserWithKeycloak()
        {
            var result = await SyncProcess();
            return result ? Ok("Users synced successfully.") : StatusCode(500, "Error syncing users.");
        }

        internal async Task<bool> SyncProcess()
        {
            try
            {
                _logger.LogInformation("Starting user sync process...");

                var keycloakUsers = await GetUsersFromKeycloak();
                if (keycloakUsers == null || keycloakUsers.Count == 0)
                {
                    _logger.LogWarning("No users found in Keycloak.");
                    return false;
                }

                var existingUsernames = await GetAllUsernamesFromSqlServer();

                foreach (var user in keycloakUsers)
                {
                    var isSuccess = await SyncUserToSqlServer(user);
                    if (!isSuccess)
                    {
                        _logger.LogError($"Error syncing user {user.Username} with SQL Server.");
                        return false;
                    }

                    var normalized = (user.Username ?? "").Trim().ToLower();
                    existingUsernames.Remove(normalized);
                }

                foreach (var usernameToDelete in existingUsernames)
                {
                    await DeleteUserFromSqlServer(usernameToDelete);
                    _logger.LogInformation($"Deleted user {usernameToDelete} from SQL Server because it's not in Keycloak.");
                }

                _logger.LogInformation("Users synced successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in SyncProcess: {ex.Message}");
                return false;
            }
        }

        private async Task<string> GetAdminToken()
        {
            try
            {
                var tokenEndpoint = "http://192.168.164.145:8080/realms/test_client/protocol/openid-connect/token";

                var parameters = new Dictionary<string, string>
                {
                    { "client_id", "TestSSO" },
                    { "username", "admin" },
                    { "password", "admin" },
                    { "grant_type", "password" },
                    { "client_secret", "admin-cli-secret" }
                };

                using var requestContent = new FormUrlEncodedContent(parameters);
                using var response = await _httpClient.PostAsync(tokenEndpoint, requestContent);

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadAsStringAsync();
                    var tokenData = JsonConvert.DeserializeObject<TokenResponse1>(tokenResponse);
                    return tokenData?.Access_token ?? string.Empty;
                }

                _logger.LogError($"Failed to get Keycloak admin token. Status: {response.StatusCode}");
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in GetAdminToken: {ex.Message}");
                return string.Empty;
            }
        }

        private async Task<List<KeycloakUser>> GetUsersFromKeycloak()
        {
            try
            {
                var adminToken = await GetAdminToken();
                var keycloakApiUrl = "http://192.168.164.145:8080/admin/realms/test_client/users?briefRepresentation=false";

                var request = new HttpRequestMessage(HttpMethod.Get, keycloakApiUrl);
                request.Headers.Add("Authorization", $"Bearer {adminToken}");

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<KeycloakUser>>(content);
                }

                _logger.LogError($"Failed to fetch users from Keycloak. Status Code: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in GetUsersFromKeycloak: {ex.Message}");
                return null;
            }
        }

        private async Task<HashSet<string>> GetAllUsernamesFromSqlServer()
        {
            var usernames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var connectionString = "Server=192.168.164.145,1433;Database=keycloak_db;User Id=sa;Password=@Abc12345;TrustServerCertificate=True;";

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            connection.ChangeDatabase("keycloak_db");

            var query = "SELECT username FROM users;";
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                usernames.Add(reader.GetString(0).Trim().ToLower());
            }
            return usernames;
        }

        private async Task DeleteUserFromSqlServer(string username)
        {
            var connectionString = "Server=192.168.164.145,1433;Database=keycloak_db;User Id=sa;Password=@Abc12345;TrustServerCertificate=True;";

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            connection.ChangeDatabase("keycloak_db");

            var query = "DELETE FROM users WHERE LOWER(TRIM(username)) = @Username;";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username.Trim().ToLower());
            await command.ExecuteNonQueryAsync();
        }

        private async Task<bool> SyncUserToSqlServer(KeycloakUser user)
        {
            try
            {
                var connectionString = "Server=192.168.164.145,1433;Database=keycloak_db;User Id=sa;Password=@Abc12345;TrustServerCertificate=True;";
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var createDatabaseQuery = "IF DB_ID('keycloak_db') IS NULL CREATE DATABASE keycloak_db;";
                using (var createDbCmd = new SqlCommand(createDatabaseQuery, connection))
                {
                    await createDbCmd.ExecuteNonQueryAsync();
                }

                connection.ChangeDatabase("keycloak_db");

                var createTableQuery = @"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='users' and xtype='U')
                    CREATE TABLE users (
                        id INT IDENTITY(1,1) PRIMARY KEY,
                        username NVARCHAR(255) UNIQUE NOT NULL,
                        email NVARCHAR(255) UNIQUE NOT NULL,
                        first_name NVARCHAR(255),
                        last_name NVARCHAR(255),
                        cccd NVARCHAR(255) NULL,
                        sdt NVARCHAR(255) NULL,
                        created_at DATETIME DEFAULT GETDATE(),
                        updated_at DATETIME DEFAULT GETDATE()
                    );";

                using (var createTableCmd = new SqlCommand(createTableQuery, connection))
                {
                    await createTableCmd.ExecuteNonQueryAsync();
                }

                var query = @"
                    MERGE users AS target
                    USING (SELECT @Username AS username) AS source
                    ON target.username = source.username
                    WHEN MATCHED THEN
                        UPDATE SET 
                            email = @Email,
                            first_name = @FirstName,
                            last_name = @LastName,
                            cccd = @CCCD,
                            sdt = @SDT,
                            updated_at = GETDATE()
                    WHEN NOT MATCHED THEN
                        INSERT (username, email, first_name, last_name, cccd, sdt)
                        VALUES (@Username, @Email, @FirstName, @LastName, @CCCD, @SDT);";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", user.Username ?? string.Empty);
                command.Parameters.AddWithValue("@Email", user.Email ?? string.Empty);
                command.Parameters.AddWithValue("@FirstName", user.FirstName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@LastName", user.LastName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CCCD", string.IsNullOrWhiteSpace(user.CCCD) ? DBNull.Value : (object)user.CCCD);
                command.Parameters.AddWithValue("@SDT", string.IsNullOrWhiteSpace(user.SDT) ? DBNull.Value : (object)user.SDT);

                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error syncing user {user.Username}: {ex.Message}");
                return false;
            }
        }
    }

    public class KeycloakUser
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Dictionary<string, List<string>> Attributes { get; set; }

        public string CCCD => Attributes != null && Attributes.ContainsKey("CCCD") && Attributes["CCCD"].Count > 0 ? Attributes["CCCD"][0] : null;
        public string SDT => Attributes != null && Attributes.ContainsKey("SDT") && Attributes["SDT"].Count > 0 ? Attributes["SDT"][0] : null;
    }

    public class TokenResponse1
    {
        public string Access_token { get; set; }
    }

    // Background Service chạy auto mỗi 1 phút
    public class UserSyncBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<UserSyncBackgroundService> _logger;

        public UserSyncBackgroundService(IServiceProvider services, ILogger<UserSyncBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var controller = scope.ServiceProvider.GetRequiredService<UserUpdateController>();
                    try
                    {
                        _logger.LogInformation("Auto-sync starting at: {time}", DateTimeOffset.Now);
                        await controller.SyncProcess();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error during auto-sync");
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
