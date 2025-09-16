using Microsoft.AspNetCore.Mvc;
using api.Services.GeneralAPIServices;
namespace api.Controllers
{
    [Route("api/user")]
    public class UserController : APIController<tbUser>
    {
        public UserController(IAPIService<tbUser> repository) : base(repository)
        {
        }
    }

}