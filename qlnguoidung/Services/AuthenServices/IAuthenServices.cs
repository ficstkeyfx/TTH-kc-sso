using QLNguoiDung.Models;
namespace QLNguoiDung.Services.AuthenServices
{
    public interface IAuthenServices
    {
      Task<string> GetCurrentUserTokenAsync();
      string GetCurrentToken();
      Task<tbUser> GetMe();
    }
}

