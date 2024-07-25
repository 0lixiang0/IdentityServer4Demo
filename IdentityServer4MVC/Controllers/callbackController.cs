using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using static System.Formats.Asn1.AsnWriter;
using System.Security.Claims;

namespace IdentityServer4MVC.Controllers
{
    public class callbackController : Controller
    {
        //https://localhost:4001/callback?code=AF992EA70211EBD8E405E809A1F6FBCC23B117FE92796D97517FD06E6C755E2A&scope=openid%20profile%20simple_api%20email&session_state=bc8fuCOy11D6FQGhBeKMVC0MuDuHmEWdQh-znohjDH4.FCD837C3285DC4ED455C59C16E6FB70B
        public IActionResult Index(string code,string scope, string session_state)
        {
            var result = GetTokenAsync(code).Result;
            //拿到code以后就可以去获取用户令牌了
            return View(result);
        }


        public async Task<UserInfoResult> GetTokenAsync(string authorizationCode)
        {
            HttpClient client = new HttpClient();
            var tokenEndpoint = "https://localhost:5000/connect/token";
            var client_id = "simple_mvc_client";
            var client_secret = "simple_client_secret";
            string redirectUri = "https://localhost:4001/callback";

            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", authorizationCode),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("client_id", client_id),
                new KeyValuePair<string, string>("client_secret", client_secret)
            });

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            var tokenResult = JsonConvert.DeserializeObject<TokenResult>(responseContent);

            //var t = ParseJwt(tokenResult.id_token);
            //验证Id_token
            await ValidateTokenAsync(tokenResult.id_token, null);
            //验证acccess_token
            //await  ValidateTokenAsync(null,tokenResult.access_token);

            //await RefreshTokenAsync(tokenResult.refresh_token);

            //拿到用户信息
            var userInfo = await GetUserInfoAsync(tokenResult.access_token);
            return userInfo;
        }

        public async Task<UserInfoResult> GetUserInfoAsync(string accessToken)
        {
            HttpClient client = new HttpClient();
            var userInfoEndpoint = "https://localhost:5000/connect/userinfo";

            var request = new HttpRequestMessage(HttpMethod.Get, userInfoEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<UserInfoResult>(responseContent);
        }

        /// <summary>
        /// 解析JWT
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public static JwtSecurityToken ParseJwt(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // 读取并解析 JWT
            var jwt = tokenHandler.ReadJwtToken(jwtToken);

            // 返回解析后的 JWT
            return jwt;
        }

        /// <summary>
        /// 验证id_token的合法性
        /// </summary>
        /// <param name="idToken"></param>
        /// <returns></returns>
        public async Task ValidateTokenAsync(string idToken,string accessToken)
        {
            string validateToken = !string.IsNullOrWhiteSpace(idToken) ? idToken : accessToken;
            var authority = "https://localhost:5000";
            var clientId = !string.IsNullOrWhiteSpace(idToken)?"simple_mvc_client": "https://localhost:5000/resources";

            // 配置管理器，用于从授权服务器获取签名密钥
            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{authority}/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever());

            // 获取 OpenID Connect 配置，其中包含 JWKS URL
            var openIdConfig = await configurationManager.GetConfigurationAsync();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = openIdConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = clientId,
                ValidateLifetime = true,
                RequireSignedTokens = true,
                IssuerSigningKeys = openIdConfig.SigningKeys,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;

            try
            {
                // 验证令牌
                var principal = tokenHandler.ValidateToken(validateToken, validationParameters, out validatedToken);

                // 提取用户信息
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = principal.FindFirst(ClaimTypes.Email)?.Value;

                // 使用用户信息...
            }
            catch (Exception ex)
            {
                // 处理验证失败
                Console.WriteLine($"Token validation failed: {ex.Message}");
            }
        }

        //刷新Token
        public async Task<string> RefreshTokenAsync(string refreshToken)
        {

            HttpClient client = new HttpClient();
            var tokenEndpoint = "https://localhost:5000/connect/token";
            var client_id = "simple_mvc_client";
            var client_secret = "simple_client_secret";

            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("client_id", client_id),
                new KeyValuePair<string, string>("client_secret", client_secret),
            });

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;

        }



        public class TokenResult { 
        
            /// <summary>
            /// 用户身份令牌
            /// </summary>
            public string? id_token { get; set; }

            /// <summary>
            /// 访问令牌
            /// </summary>
            public string? access_token { get; set; }

            /// <summary>
            /// 刷新令牌
            /// </summary>
            public string? refresh_token { get; set; }

            /// <summary>
            /// 过期时间
            /// </summary>
            public int expires_in { get; set; }

            /// <summary>
            /// 令牌类型
            /// </summary>
            public string? token_type { get; set; }

            /// <summary>
            /// 权限
            /// </summary>
            public string? scope { get; set; }
        }

        public class UserInfoResult
        {
            /// <summary>
            /// 用户身份令牌
            /// </summary>
            public string? name { get; set; }

            /// <summary>
            /// 访问令牌
            /// </summary>
            public string? email { get; set; }

            /// <summary>
            /// 过期时间
            /// </summary>
            public string? website { get; set; }

            /// <summary>
            /// 令牌类型
            /// </summary>
            public string? sub { get; set; }

        }
    }
}
