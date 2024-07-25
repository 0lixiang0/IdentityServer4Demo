using IdentityServer4Demo;
using IdentityServer4;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4Demo.Model;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
});



builder.Services.AddControllersWithViews();
//builder.Services.AddIdentityServer(options =>
//        {
//            options.Events.RaiseErrorEvents = true;
//            options.Events.RaiseInformationEvents = true;
//            options.Events.RaiseFailureEvents = true;
//            options.Events.RaiseSuccessEvents = true;
//            options.EmitStaticAudienceClaim = true;
//        })
//        .AddDeveloperSigningCredential()  //默认的生成的密钥（运行后，会在项目根目录下生成文件 tempkey.jwk）
//        .AddInMemoryClients(Config.Clients) //注册客户端
//        .AddInMemoryApiScopes(Config.ApiScopes) //注册api访问范围
//        .AddTestUsers(Config.Users) //注册资源拥有者
//        .AddInMemoryIdentityResources(Config.IdentityResources); //用户的身份资源信息（例如：显示昵称，头像，等等信息）



string migrationsAssembly = Assembly.GetExecutingAssembly().GetName().Name;
const string connectionString = "Data Source=.;Initial Catalog=IdentityServer4Demo;Integrated Security=True;TrustServerCertificate=yes;";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddIdentityServer()
        .AddDeveloperSigningCredential()  //默认的生成的密钥（运行后，会在项目根目录下生成文件 tempkey.jwk）
        .AddAspNetIdentity<ApplicationUser>()
        .AddConfigurationStore(options =>
        {
            options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                sql => sql.MigrationsAssembly(migrationsAssembly));
        })
        .AddOperationalStore(options =>
        {
            options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                sql => sql.MigrationsAssembly(migrationsAssembly));
        });

var app = builder.Build();
//app.InitIdentityServer4Db();   //第一次启动程序初始化数据的时候需要放开此注释
//app.InitAspnetIdentityDb();   //第一次启动程序初始化数据的时候需要放开此注释
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
