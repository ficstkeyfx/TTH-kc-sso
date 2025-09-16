using Microsoft.AspNetCore.Mvc;
using api.Services.GeneralAPIServices;
namespace api.Controllers
{
    [Route("api/chucnang")]
    public class ChucNangController : APIController<tbChucNang>
    {
        public ChucNangController(IAPIService<tbChucNang> repository) : base(repository)
        {
        }
    }

}