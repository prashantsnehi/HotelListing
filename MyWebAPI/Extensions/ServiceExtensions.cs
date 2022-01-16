using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MyWebAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using MyWebAPI.Models;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Marvin.Cache.Headers;
using Microsoft.OpenApi.Models;

namespace MyWebAPI.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureIdentity(this IServiceCollection service)
        {
            var builder = service.AddIdentityCore<ApiUser>(x => x.User.RequireUniqueEmail = true);
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), service);

            // Refresh Token
            builder.AddTokenProvider("MyWebAPI", typeof(DataProtectorTokenProvider<ApiUser>));
            builder.AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();
        }

        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(error => {
                error.Run(async context => {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if(contextFeature != null) {
                        Log.Error($"Something went wrong {contextFeature.Error}");

                        await context.Response.WriteAsync(new Error {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server Error. Please try again later"
                        }.ToString());
                    }
                });
            });
        }

        public static void ConfigureVersioning(this IServiceCollection service) 
        {
            service.AddApiVersioning(opt => 
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });
        }

        public static void ConfigureHttpCacheHeaders(this IServiceCollection service)
        {
            service.AddResponseCaching();
            service.AddHttpCacheHeaders(
                (expirationOpt) => 
                {
                    expirationOpt.MaxAge = 65;
                    expirationOpt.CacheLocation = CacheLocation.Private;
                },
                (validationOpt) => 
                {
                    validationOpt.MustRevalidate = true;
                }
            );
        }

        public static void ConfigureRateLimit(this IServiceCollection service)
        {
            var rateLimitRules = new List<RateLimitRule>
            {
                new RateLimitRule
                {
                    Endpoint = "*",
                    Limit = 1,
                    Period = "5s"
                }
            }; 

            service.Configure<IpRateLimitOptions>(opt => 
            {
                opt.GeneralRules = rateLimitRules;
            });

            service.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            service.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            service.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        public static void ConfigureSwaggerDoc(this IServiceCollection service)
        {
            service.AddSwaggerGen(c => 
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "HotelListing",
                    Version = "v1",
                    Description = "Learning REST API using ASP.NET Core 5.0",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Prashant",
                        Email = "prashant@tsiplc.com"
                    }
                });
                
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "0auth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
        }
    }
}
