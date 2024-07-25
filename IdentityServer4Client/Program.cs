using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{

    // 配置 Swagger UI 中的身份验证
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });
});

builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", p =>
{
    //oidc的服务地址(一定要用https!!)
    p.Authority = "https://localhost:5000";//也就是IdentifyServer项目运行地址                                    //设置jwt的验证参数(默认情况下是不需要此验证的)
    p.MetadataAddress = "https://localhost:5000/.well-known/openid-configuration";
    p.RequireHttpsMetadata=true;
    p.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
});
//注册授权服务
builder.Services.AddAuthorization(p =>
{
    //添加授权策略
    p.AddPolicy("MyApiScope", opt =>
    {
        //配置鉴定用户的规则，也就是说必须通过身份认证
        opt.RequireAuthenticatedUser();
        //鉴定api范围的规则
        opt.RequireClaim("scope", "simple_api");
    });

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
