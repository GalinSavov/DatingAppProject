using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        //set up ASP.NET Identity
        services.AddIdentityCore<AppUser>()
        .AddRoles<AppRole>()
        .AddRoleManager<RoleManager<AppRole>>()
        .AddEntityFrameworkStores<DataContext>();

        //sets the default authentication to be JWT Bearer token
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => //configure how the JWT token is validated
        {
            var tokenKey = configuration["TokenKey"] ?? throw new Exception("TokenKey not found!"); //get the env variable TokenKey from appsettings.json
            options.TokenValidationParameters = new TokenValidationParameters // how to validate if the token is valid
            {
                ValidateIssuerSigningKey = true, //the token’s signature must match the server’s secret key
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)), //encode TokenKey
                ValidateIssuer = false,
                ValidateAudience = false,
            };
            options.Events = new JwtBearerEvents   //SignalR support, allows the token to be sent via query string when connecting to /hubs
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(path) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });
        services.AddAuthorizationBuilder()
        .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
        .AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));

        return services;
    }
}