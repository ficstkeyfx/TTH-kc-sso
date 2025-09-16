using Microsoft.AspNetCore.Mvc;
using api.Services.GeneralAPIServices;
namespace api.Controllers
{
    [Route("api/nhom")]
    public class NhomController : APIController<tbNhom>
    {
        public NhomController(IAPIService<tbNhom> repository) : base(repository)
        {
        }
    }

}