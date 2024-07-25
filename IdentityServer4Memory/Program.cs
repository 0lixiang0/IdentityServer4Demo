using IdentityServer4Demo;
using IdentityServer4;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using IdentityServer4DemoMemory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
});



builder.Services.AddControllersWithViews();
builder.Services.AddIdentityServer(options =>
        {
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;
            options.EmitStaticAudienceClaim = true;
        })
        .AddDeveloperSigningCredential()  //默认的生成的密钥（运行后，会在项目根目录下生成文件 tempkey.jwk）
        .AddInMemoryClients(Config.Clients) //注册客户端
        .AddInMemoryApiScopes(Config.ApiScopes) //注册api访问范围
        .AddTestUsers(Config.Users) //注册资源拥有者
        .AddInMemoryIdentityResources(Config.IdentityResources); //用户的身份资源信息（例如：显示昵称，头像，等等信息）
        //.AddProfileService<CustomProfileService>();


var app = builder.Build();
app.UseCookiePolicy();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();
app.UseEndpoints(endpoint =>
{
    endpoint.MapDefaultControllerRoute();
});


app.Run();
