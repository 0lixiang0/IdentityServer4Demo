using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.DataProtection;
using System.Net.Sockets;
using System.Security.Claims;
using Secret = IdentityServer4.Models.Secret;

namespace IdentityServer4Demo
{
    /// <summary>
    /// IdentityServer4配置类
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Api范围
        /// </summary>
        public static IEnumerable<ApiScope> ApiScopes => new[]
        {
            new ApiScope()
            {
                Name = "simple_api",
                DisplayName = "Simple_API"
            }
        };

        public static IEnumerable<Client> Clients => new[]
        {
            //客户端模式
            new Client()
            {
                ClientId = "simple_client",
                ClientSecrets = new List<Secret>()
                {
                    new Secret("simple_client_secret".Sha256())
                },
                AllowedGrantTypes = new List<string>(){GrantType.ClientCredentials},
                AllowedScopes = { "simple_api" }//允许访问的api,可以有多个
            },
            //用户名密码模式
            new Client()
            {
                ClientId = "simple_pass_client",
                ClientSecrets = new List<Secret>()
                {
                    new Secret("simple_client_secret".Sha256())
                },
                AllowedGrantTypes = new List<string>(){GrantType.ResourceOwnerPassword},
                AllowedScopes = { "simple_api" }//允许访问的api,可以有多个
            },
            //授权码模式
            new Client()
            {
                ProtocolType=IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                ClientId = "simple_mvc_client",
                ClientName = "Simple Mvc Client",
                ClientSecrets = new List<Secret>()
                {
                    new Secret("simple_client_secret".Sha256())
                },
                AllowedGrantTypes = new List<string>(){GrantType.AuthorizationCode}, //授权码许可协议
                //登录成功的跳转地址(坑点：必须使用Https！！！)
                RedirectUris = {"https://localhost:4001/callback"}, 
                //登出后的跳转地址
                PostLogoutRedirectUris = {"https://localhost:4001/Home/logout-callback"},
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile, //IdentityResources中定义了这两项，在这里也需要声明
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    "simple_api"
                },//允许访问的api,可以有多个
                RequireConsent = true, //是否需要用户点同意
                RequirePkce = false,
                AllowOfflineAccess=true
            },
             //OIDC模式
            new Client()
            {
                ProtocolType=IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                ClientId = "simple_oidc_client",
                ClientName = "Simple OIDC Client",
                ClientSecrets = new List<Secret>()
                {
                    new Secret("simple_oidc_secret".Sha256())
                },
                AllowedGrantTypes = new List<string>(){GrantType.Hybrid}, //授权码许可协议
                //登录成功的跳转地址(坑点：必须使用Https！！！)
                RedirectUris = {"https://localhost:7099/signin-oidc"}, //mvc客户端的地址，signin-oidc:标准协议里的端点名称
                //登出后的跳转地址
                PostLogoutRedirectUris = {"https://localhost:7099/signout-callback-oidc"},
                AllowedCorsOrigins={ "https://localhost:7099"},
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile, //IdentityResources中定义了这两项，在这里也需要声明
                    "simple_api"
                },//允许访问的api,可以有多个
                RequireConsent = true, //是否需要用户点同意
                RequirePkce = false,
                AlwaysIncludeUserClaimsInIdToken = true,
                AccessTokenLifetime=31536000,
                IdentityTokenLifetime=300,
                AllowOfflineAccess = true,
            },
             //OIDC模式
            new Client()
            {
                ProtocolType=IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                ClientId = "simple_oidc_client1",
                ClientName = "Simple OIDC Client1",
                ClientSecrets = new List<Secret>()
                {
                    new Secret("simple_oidc_secret1".Sha256())
                },
                AllowedGrantTypes = new List<string>(){GrantType.Hybrid}, //授权码许可协议
                //登录成功的跳转地址(坑点：必须使用Https！！！)
                RedirectUris = {"https://localhost:7199/signin-oidc"}, //mvc客户端的地址，signin-oidc:标准协议里的端点名称
                //登出后的跳转地址
                PostLogoutRedirectUris = {"https://localhost:7199/signout-callback-oidc"},
                AllowedCorsOrigins={ "https://localhost:7199"},
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile, //IdentityResources中定义了这两项，在这里也需要声明
                    "simple_api"
                },//允许访问的api,可以有多个
                RequireConsent = true, //是否需要用户点同意
                RequirePkce = false,
                AlwaysIncludeUserClaimsInIdToken = true,
                AccessTokenLifetime=31536000,
                IdentityTokenLifetime=300,
                AllowOfflineAccess = true,
            },
        };
        //资源拥有者(TestUser只是IdentifyServer4提供的一个测试用户)
        public static List<TestUser> Users = new List<TestUser>()
        {
            new TestUser()
            {
                SubjectId = "1",
                Username = "admin",
                Password = "123",
                Claims = new List<Claim>
                {
                    new Claim("name", "Bob"),
                    new Claim("email", "bob@example.com"),
                    new Claim("website", "https://bob.com"),
                    new Claim("role", "user")
                }
            }
        };

        //定义身份资源（标准的oidc(OpenId Connect)）
        public static IEnumerable<IdentityResource> IdentityResources = new List<IdentityResource>()
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),//档案信息(昵称，头像。。。)
            new IdentityResources.Address(),
            new IdentityResources.Email(),
            new IdentityResources.Phone()
        };
    }
}
