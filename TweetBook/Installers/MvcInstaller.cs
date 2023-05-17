using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TweetBook.Options;
using System.Security.Cryptography.Xml;
using TweetBook.Services;
using TweetBook.Authorization;
using Microsoft.AspNetCore.Authorization;
using FluentValidation.AspNetCore;
using System.IO.Compression;
using TweetBook.Filters;

namespace TweetBook.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IConfiguration configuration, IServiceCollection services)
        {

             //Token Service

            var jwtSettings = new JwtSettings();
            configuration.GetSection("JwtSettings").Bind(jwtSettings);

            services.AddSingleton(jwtSettings);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = tokenValidationParameters;

            });


            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("TagViewer", builder =>
            //    {
            //        builder.RequireClaim("tags.view", "true");
            //    });
            //});

            services.AddFluentValidationAutoValidation();
            services.AddAutoMapper(typeof(Program));
            services.AddMvc(options => 
            {
                options.Filters.Add<ValidationFilter>();
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("MustWorkInCompany", policy =>
                {
                    policy.AddRequirements(new WorksForCompanyRequirement("gmail.com"));
                });
            });

            services.AddSingleton<IAuthorizationHandler, WorksForCompanyHandler>();

            services.AddSingleton(tokenValidationParameters);


            //Identity Service

            services.AddScoped<IIdentityService, IdentityService>();


            //Uri service for pagination

            services.AddSingleton<IUriService>(provider =>
            {
                var accessor =provider.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var absoluteUri = string.Concat(request.Scheme,"://",request.Host.ToUriComponent(),"/");
                return new UriService(absoluteUri);
            });
            


        }
    }
}
