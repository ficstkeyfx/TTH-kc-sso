using Microsoft.AspNetCore.Mvc;
using api.Services.GeneralAPIServices;
namespace api.Controllers
{
    [Route("api/chucnang_nhom")]
    public class ChucNang_NhomController : APIController<tbChucNang_Nhom>
    {
        public ChucNang_NhomController(IAPIService<tbChucNang_Nhom> repository) : base(repository)
        {
        }
    }

}