using Microsoft.AspNetCore.Mvc;
using api.Services.GeneralAPIServices;
namespace api.Controllers
{
    [Route("api/systemuser")]
    public class SystemUserController : APIController<tbSystemUser>
    {
        public SystemUserController(IAPIService<tbSystemUser> repository) : base(repository)
        {
        }
    }

}