using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace api.Services.TVFServices
{
    public class TVFServices : ITVFServices
    {
        private readonly dbAPIContext _context;

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TVFServices(dbAPIContext accountContext, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _configuration = configuration;
            _context = accountContext;
            _httpContextAccessor = contextAccessor;

        }
        public async Task<ServiceResponse<List<tbChucNang>>> getChucNang(int IdUser)
        {
            var response = new ServiceResponse<List<tbChucNang>>();
            //var usrChucNang = await _context.getUserChucNang(1).ToListAsync();
            var usrChucNang = await _context.tbChucNangs
            .FromSqlRaw("SELECT * FROM getChucNang(" + IdUser + ")")
            .ToListAsync();
            response.Data = usrChucNang;
            return response;
        }
        public async Task<ServiceResponse<List<tbUser>>> getNguoiDungThuocNhom(int IdNhom)
        {
            var response = new ServiceResponse<List<tbUser>>();
            var nguoiDungThuocNhom = await _context.tbUsers
            .FromSqlRaw("SELECT * FROM getNguoiDungThuocNhom(" + IdNhom + ")")
            .ToListAsync();
            response.Data = nguoiDungThuocNhom;
            return response;
        }
    }
}