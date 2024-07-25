using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Security.Claims;

namespace IdentityServer4DemoMemory
{
    public class CustomProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, context.Subject.GetSubjectId()),
                new Claim(JwtClaimTypes.GivenName, "John"),
                new Claim(JwtClaimTypes.FamilyName, "Doe"),
                new Claim(JwtClaimTypes.Email, "bob@example.com"),
                // 其他需要返回的用户信息
            };

            context.IssuedClaims.AddRange(claims);
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true; // 这里假设用户总是活跃
            return Task.CompletedTask;
        }
    }
}
