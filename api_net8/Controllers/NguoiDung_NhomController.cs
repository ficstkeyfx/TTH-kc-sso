using Microsoft.AspNetCore.Mvc;
using api.Services.GeneralAPIServices;
namespace api.Controllers
{
    [Route("api/nguoidung_nhom")]
    public class NguoiDung_NhomController : APIController<tbNguoiDung_Nhom>
    {
        public NguoiDung_NhomController(IAPIService<tbNguoiDung_Nhom> repository) : base(repository)
        {
        }
    }

}