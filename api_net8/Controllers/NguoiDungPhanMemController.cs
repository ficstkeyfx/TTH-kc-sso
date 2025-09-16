using Microsoft.AspNetCore.Mvc;
using api.Services.GeneralAPIServices;
namespace api.Controllers
{
    [Route("api/nguoidungphanmem")]
    public class NguoiDungPhanMemController : APIController<vNguoiDungPhanMem>
    {
        public NguoiDungPhanMemController(IAPIService<vNguoiDungPhanMem> repository) : base(repository)
        {
        }
    }

}