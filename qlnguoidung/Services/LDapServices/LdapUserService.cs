using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;

namespace QLNguoiDung.Services.LDapServices
{
    public class LdapUserService
    {
        private readonly string _domain = "C500.edu.vn";
        //private readonly string _container = "OU=Users,DC=yourdomain,DC=com"; // Thay đổi theo tổ chức của bạn
        private readonly string _adminUsername = "nghiand";
        private readonly string _adminPassword = "@Abc123";

        public void CreateUser(string username, string password, string displayName, string email)
        {
            using (var context = new PrincipalContext(ContextType.Domain, _domain, _adminUsername, _adminPassword))
            {
                using (var user = new UserPrincipal(context))
                {
                    user.SamAccountName = username;
                    user.UserPrincipalName = $"{username}@{_domain}";
                    user.DisplayName = displayName;
                    user.EmailAddress = email;
                    user.SetPassword(password);
                    user.Enabled = true;
                    user.ExpirePasswordNow(); // Bắt buộc đổi mật khẩu khi đăng nhập nếu cần

                    try
                    {
                        user.Save();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Không thể tạo tài khoản LDAP: " + ex.Message, ex);
                    }
                }
            }
        }
    }
}
