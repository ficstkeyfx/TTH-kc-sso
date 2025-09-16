namespace api.Services.AuthServices
{
    public interface IAuthServices
    {
        Task<ServiceResponse<string>> Login(string username, string password);
        Task<bool> UserExists(string username);
        Task<ServiceResponse<tbUser>> Me();
    }
}