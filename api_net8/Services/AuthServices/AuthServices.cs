using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace api.Services.AuthServices
{
    public class AuthServices : IAuthServices
    {
        private readonly dbAPIContext _context;

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthServices(dbAPIContext accountContext, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _configuration = configuration;
            _context = accountContext;
            _httpContextAccessor = contextAccessor;

        }
        //private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private int? GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdClaim, out int userId))
                    return userId;
            }
            return null; 
        }

        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.tbUsers
                .FirstOrDefaultAsync(u => u.UserName.ToLower().Equals(username.ToLower()));
            if (user is null)
            {
                response.Success = false;
                response.Message = "Người dùng không tồn tại";
            }
            else
            {
                response.Data = CreateToken(user);
            }
            return response;
        }



        public async Task<bool> UserExists(string username)
        {
            if (await _context.tbUsers.AnyAsync(u => u.UserName.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        public async Task<ServiceResponse<tbUser>> Me()
        {

            var response = new ServiceResponse<tbUser>();
            try
            {
                tbUser data = await _context.tbUsers.Where(p=>p.IdUser==GetUserId()).FirstOrDefaultAsync();
                if (data is null)
                {
                    throw new Exception($"User không tồn tại");
                }
                response.Data = data;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;

            }
            return response;

        }

        private string CreateToken(tbUser user)
        {
            var claims = new List<Claim>
           {
               new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString() ?? "0"),
               new Claim(ClaimTypes.Name, user.UserName),
           };

            var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
            if (appSettingsToken is null)
                throw new Exception("AppSettings Token is null!");

            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(appSettingsToken));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}