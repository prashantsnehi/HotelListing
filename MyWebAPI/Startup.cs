using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyWebAPI.Configurations;
using MyWebAPI.Data;
using MyWebAPI.Extensions;
using MyWebAPI.IRepository;
using MyWebAPI.Repository;
using MyWebAPI.Services;

namespace MyWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddControllers();

            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("sqlConnection"))
            );

            services.ConfigureHttpCacheHeaders();
            
            services.AddMemoryCache();
            services.ConfigureRateLimit();
            services.AddHttpContextAccessor();

            services.AddAuthentication();

            services.ConfigureIdentity();
            services.ConfigureJwt(Configuration);

            services.AddCors(policy =>
            {
                policy.AddPolicy("CoresPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            services.AddAutoMapper(typeof(MapperInitilizer));

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthManager, AuthManager>();

            services.ConfigureSwaggerDoc();
            // services.AddSwaggerGen(options =>
            // {
            //     options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            //     {
            //         Title = "HotelListing",
            //         Version = "v1",
            //         Description = "Learning REST API using ASP.NET Core 5.0",
            //         Contact = new Microsoft.OpenApi.Models.OpenApiContact
            //         {
            //             Name = "Prashant",
            //             Email = "prashant@tsiplc.com"
            //         }
            //     });
            // });

            services.AddControllers(config => {
                config.CacheProfiles.Add("120SecondsDuration", new CacheProfile
                {
                    Duration = 120
                });
            }).AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelListing v1"));

            app.UseHttpsRedirection();

            app.UseCors("CoresPolicy");
            app.UseResponseCaching();
            app.UseHttpCacheHeaders();
            app.UseIpRateLimiting();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
