using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

//JwtSecurityTokenHandler.DefaultMapInboundClaims=false;


//builder.Services.Configure<CookiePolicyOptions>(option =>
//{
//    option.MinimumSameSitePolicy = SameSiteMode.Strict;
//    option.Secure = CookieSecurePolicy.None;
//});

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme=CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme=OpenIdConnectDefaults.AuthenticationScheme;
//})
//.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
//.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
//{
//    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.Authority = "https://localhost:5000";//也就是IdentifyServer项目运行地址
//    options.ClientId="simple_oidc_client";
//    options.ClientSecret="simple_oidc_secret";
//    options.RequireHttpsMetadata=true;
//    options.ResponseType=OpenIdConnectResponseType.CodeIdToken;
//    options.GetClaimsFromUserInfoEndpoint=true;
//    options.UsePkce=false;
//    options.SaveTokens=true;
//    options.Scope.Add("openid");
//    options.Scope.Add("profile");
//    options.Scope.Add("simple_api");

//    //必须设置cookie signin-oidc 返回的cookie设置
//    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.None;
//    options.NonceCookie.SecurePolicy = CookieSecurePolicy.None;
//    options.CorrelationCookie.SameSite = SameSiteMode.Strict;
//    options.NonceCookie.SameSite = SameSiteMode.Strict;

//    options.Events = new OpenIdConnectEvents
//    {
//        OnAuthorizationCodeReceived = async context =>
//        {
//            var request = context.HttpContext.Request;
//            var response = context.HttpContext.Response;
//            var session = context.HttpContext.Session;

//            // 交换授权码获取令牌
//            var tokenResponse = await context.HttpContext.GetTokenAsync(OpenIdConnectDefaults.AuthenticationScheme, "access_token");
//            //var refreshToken = await context.HttpContext.GetTokenAsync(OpenIdConnectDefaults.AuthenticationScheme, "refresh_token");

//            // 将令牌存储在服务器端会话中
//            session.SetString("access_token", tokenResponse);
//            //session.SetString("refresh_token", refreshToken);
//        }
//    };
//});

//builder.Services.AddDistributedMemoryCache();
//builder.Services.AddSession(options =>
//{
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();
//app.UseAuthorization();
app.UseAuthentication();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
