// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4Demo.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityServer4Demo
{
    public static class SeedData
    {
        public static void InitIdentityServer4Db(this IApplicationBuilder app) {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.IdentityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in Config.ApiScopes)
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

            }
        }


        public static void InitAspnetIdentityDb(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();

                //获取用户服务
                var userManage = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var admin =  userManage.FindByNameAsync("admin").Result;
                if (admin==null)
                {
                    admin=new ApplicationUser() { 
                        UserName = "admin",
                        RealName = "Administrator",
                        Email = "admin@email.com",
                        EmailConfirmed=true
                    };

                    //密码必须合规则
                    var result = userManage.CreateAsync(admin, "ABCabc123!").Result;
                    if (!result.Succeeded) {
                        throw new Exception(result.Errors.FirstOrDefault().Description);
                    }

                    //添加其他身份信息
                    result = userManage.AddClaimsAsync(admin, new List<Claim>() {
                        new Claim(JwtClaimTypes.Name,"Administrator"),
                        new Claim(JwtClaimTypes.WebSite,"http://xxx.net")
                    }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.FirstOrDefault().Description);
                    }
                }

            }
        }

    }
}
