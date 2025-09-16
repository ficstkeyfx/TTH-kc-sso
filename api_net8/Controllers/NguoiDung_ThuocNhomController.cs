using api.Dtos.User;
using api.Services.AuthServices;
using api.Services.TVFServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/nguoidung_thuocnhom")]
    public class NguoiDung_ThuocNhomController : ControllerBase
    {
        private readonly ITVFServices _repo;

        public NguoiDung_ThuocNhomController(ITVFServices repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<tbUser>>> getNguoiDungThuocNhom(int IdNhom)
        {
            var response = await _repo.getNguoiDungThuocNhom(IdNhom);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}