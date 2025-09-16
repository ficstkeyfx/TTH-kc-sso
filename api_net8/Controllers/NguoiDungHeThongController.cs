using Microsoft.AspNetCore.Mvc;
using api.Services.GeneralAPIServices;
namespace api.Controllers
{
    [Route("api/nguoidunghethong")]
    public class NguoiDungHeThongController : APIController<vNguoiDungHeThong>
    {
        public NguoiDungHeThongController(IAPIService<vNguoiDungHeThong> repository) : base(repository)
        {
        }
    }

}