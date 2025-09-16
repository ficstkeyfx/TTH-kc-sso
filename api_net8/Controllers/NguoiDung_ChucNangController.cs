using api.Dtos.User;
using api.Services.AuthServices;
using api.Services.TVFServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/nguoidung_chucnang")]
    public class NguoiDung_ChucNangController : ControllerBase
    {
        private readonly ITVFServices _repo;

        public NguoiDung_ChucNangController(ITVFServices repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<tbChucNang>>> getChungNang(int IdUser)
        {
            var response = await _repo.getChucNang(IdUser);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}