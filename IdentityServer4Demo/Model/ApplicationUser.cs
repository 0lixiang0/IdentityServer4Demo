using Microsoft.AspNetCore.Identity;

namespace IdentityServer4Demo.Model
{
    public class ApplicationUser:IdentityUser
    {
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string? RealName { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int? Age { get; set; }
    }
}
