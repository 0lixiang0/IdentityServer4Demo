using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http.Headers;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultMapInboundClaims=false;
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme=CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme=OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.CallbackPath = new PathString("/signin-oidc");
    options.Authority = "https://localhost:5000";//也就是IdentifyServer项目运行地址
    options.MetadataAddress = "https://localhost:5000/.well-known/openid-configuration";
    options.ClientId="simple_oidc_client";
    options.ClientSecret="simple_oidc_secret";
    options.RequireHttpsMetadata=false;
    options.ResponseType=OpenIdConnectResponseType.CodeIdToken;
    options.GetClaimsFromUserInfoEndpoint=true;
    options.UsePkce=false;
    options.SaveTokens=true;
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("simple_api");
    options.Events=new OpenIdConnectEvents()
    {
        OnAccessDenied = context =>
        {
            context.Response.Redirect("/Account/AccessDenied");
            context.HandleResponse(); // This prevents the default behavior
            return Task.CompletedTask;
        },
        OnAuthenticationFailed=context => {

            return Task.CompletedTask;
        },
        OnAuthorizationCodeReceived=context => {

            return Task.CompletedTask;
        },
        OnTicketReceived=context => {

            return Task.CompletedTask;
        },
        OnUserInformationReceived=context => {
            return Task.CompletedTask;
        },
        OnRemoteFailure=context => {

            return Task.CompletedTask;
        },
        OnTokenValidated=context => {

            return Task.CompletedTask;
        },

    };
});

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//注意以下两个位置不能写反，写反以后会出现页面一直跳转
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
