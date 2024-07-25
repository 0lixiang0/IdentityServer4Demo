using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;

namespace IdentityServer4Client.Controllers
{
    [Authorize("MyApiScope")] //MyApiScope 这个字符串与Startup配置一致
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        [Authorize]
        [HttpGet(Name = "")]
        public IActionResult Get()
        {
            //用户信息(此时只是为了模拟返回数据，正常开发时：换成访问数据库的代码就行)
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }


        /// <summary>
        /// 创建一个token
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("CreateToken")]
        public string GetToken() {
            // 定义 token 处理程序
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("DSAFDSAFDSAFDSAFDSAFDFDSAJFDLKSAFJLDKDSAF");


            string username = "wfl";
            // 创建 token 描述符
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim("test", "test_v")
                    // 在这里你可以添加更多的声明（claims）
                }),
                Expires = DateTime.UtcNow.AddHours(1), // 设置过期时间
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // 创建 token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // 返回 JWT token 字符串
            return tokenHandler.WriteToken(token);
        }


        /// <summary>
        /// 获取一个IdentityServer4的客户端模式的令牌
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetClientToken")]
        public async Task<IdentityServer4TokenOutput> GetClientToken()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:5000");
            var formData = new Dictionary<string, string>
            {
                { "client_id", "simple_client" },
                { "client_secret", "simple_client_secret" },
                { "grant_type", "client_credentials" },
            };
            var content = new FormUrlEncodedContent(formData);
            HttpResponseMessage response = await client.PostAsync("connect/token", content);
            var result =await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IdentityServer4TokenOutput>(result);

        }


        /// <summary>
        /// 获取一个IdentityServer4的密码模式的令牌
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetPassWordToken")]
        public async Task<IdentityServer4TokenOutput> GetPassWordToken(string loginName= "admin", string password="123")
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:5000");
            var formData = new Dictionary<string, string>
            {
                { "client_id", "simple_pass_client" },
                { "client_secret", "simple_client_secret" },
                { "grant_type", "password" },
                { "username", loginName },
                { "password", password },
            };
            var content = new FormUrlEncodedContent(formData);
            HttpResponseMessage response = await client.PostAsync("connect/token", content);
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IdentityServer4TokenOutput>(result);

        }


        public class IdentityServer4TokenOutput { 

            /// <summary>
            /// 身份令牌
            /// </summary>
            public string access_token { get; set; }
            /// <summary>
            /// 过期时间
            /// </summary>
            public int expires_in { get; set; }
            /// <summary>
            /// 令牌类型
            /// </summary>
            public string token_type { get; set; }
            /// <summary>
            /// 权限
            /// </summary>
            public string scope { get; set; }
        }
    }
}
