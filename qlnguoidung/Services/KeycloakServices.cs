using System;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using System.Web;
using HtmlAgilityPack;

namespace QLNguoiDung.Services
{
    public class KeycloakServices
    {
        private const string KeycloakBase = "http://192.168.164.145:8080";
        private const string Realm = "TestSSO";
        private const string ClientId = "TestSSO3";
        private const string RedirectUri = "https://localhost:7134/callback";
        private const string AUTHORIZE_URL = $"{KeycloakBase}/realms/{Realm}/protocol/openid-connect/auth";

        /// <summary>
        /// Trả về URL để thực hiện silent authentication (check SSO mà không hiện popup đăng nhập).
        /// </summary>
        public static string UrlAuthSilent()
        {
            return $"{KeycloakBase}/realms/{Realm}/protocol/openid-connect/auth" +
                $"?client_id={ClientId}" +
                $"&redirect_uri={RedirectUri}" +
                $"&response_type=code" +
                $"&prompt=none";
        }

        public static string GetUrlLoginKC()
        {
            // Tạo một state ngẫu nhiên
            string state = Guid.NewGuid().ToString();

            // Lưu state vào session (sử dụng ASP.NET Session)
            // HttpContext.Current.Session["oidc_state"] = state;

            // Định nghĩa các tham số của URL
            // string redirectUri = "https://localhost:7134/callback";

            // Mã hóa redirectUri
            string redirectUriEncoded = HttpUtility.UrlEncode(RedirectUri);

            // Tạo URL đăng nhập với các tham số
            string authorizeUrl = $"{AUTHORIZE_URL}?client_id=TestSSO3&response_type=code&scope=openid%20profile%20email&redirect_uri={redirectUriEncoded}&state={state}";

            return authorizeUrl;
        }

        public static async Task<KCData> GetActionFormLoginKC(string url)
        {
            // Khởi tạo HttpClient
            using (var client = new HttpClient())
            {
                // Gửi yêu cầu GET đến auth_url
                HttpResponseMessage response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    // Trả về lỗi nếu không nhận được mã trạng thái 200
                    throw new Exception("Error fetching login page: " + response.StatusCode);
                }
                // Lấy cookies từ response
                var setCookies = response.Headers.Contains("Set-Cookie") 
                    ? response.Headers.GetValues("Set-Cookie").ToList()
                    : new List<string>();

                var authCookie = setCookies
                    .FirstOrDefault(c => c.StartsWith("AUTH_SESSION_ID_LEGACY=", StringComparison.OrdinalIgnoreCase));
                var kcRestartCookie = setCookies
                    .FirstOrDefault(c => c.StartsWith("KC_RESTART=", StringComparison.OrdinalIgnoreCase));

                var cookies = new Dictionary<string, string>
                {
                    { "AUTH_SESSION_ID_LEGACY", authCookie != null ? authCookie.Split(';')[0].Split('=')[1] : string.Empty },
                    { "KC_RESTART", kcRestartCookie != null ? kcRestartCookie.Split(';')[0].Split('=')[1] : string.Empty }
                };
                // Phân tích HTML bằng HtmlAgilityPack
                var doc = new HtmlDocument();
                doc.LoadHtml(await response.Content.ReadAsStringAsync());
                // Tìm form với id 'kc-form-login'
                var form = doc.DocumentNode.SelectSingleNode("//form[@id='kc-form-login']");
                if (form == null)
                {
                    throw new Exception("Login form not found in Keycloak response");
                }
                // Lấy action của form
                string action = form.GetAttributeValue("action", string.Empty);
                // Lấy các trường ẩn trong form
                var hiddenFields = new Dictionary<string, string>();
                foreach (var inputTag in form.SelectNodes(".//input[@type='hidden']"))
                {
                    string name = inputTag.GetAttributeValue("name", string.Empty);
                    string value = inputTag.GetAttributeValue("value", string.Empty);
                    if (!string.IsNullOrEmpty(name))
                    {
                        hiddenFields[name] = value;
                    }
                }
                return new KCData
                {
                    action = action,
                    authCookies = cookies,
                    hiddenFields = hiddenFields
                };
            }
        }
    }
    public class KCData
    {
        public string action { get; set; }
        public Dictionary<string, string> authCookies { get; set; }
        public Dictionary<string, string> hiddenFields { get; set; }
    }
}
