using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.Entities;
using Core.Interfaces;
using AutoMapper;
using API.Helper;
using API.Middleware;
using Microsoft.AspNetCore.Mvc;
using API.Errors;
using API.Extensions;
using StackExchange.Redis;
using Infrastructure.Identity;
using Microsoft.Extensions.FileProviders;

namespace API
{
    public class Startup
    {

        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


      
            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddControllers();
            


            //database for the store.
            services.AddDbContext<StoreContext>(x=>
              x.UseNpgsql(_config.GetConnectionString
                ("DefaultConnection")));



            //database for identity
                services.AddDbContext<AppIdentityDbContext>(x => {
                    x.UseNpgsql(_config.GetConnectionString("IdentityConnection"));
                });

             services.AddSingleton<IConnectionMultiplexer>(c => {
                 var configuration =
                 ConfigurationOptions.Parse(_config.GetConnectionString("Redis"),
                 true);
                     return ConnectionMultiplexer.Connect(configuration);
             });


            services.AddAplicationServices();
            services.AddIdentityServices(_config);
            services.AddSwaggerDocumentation();

            //agrego cors para usarlo en angular (Control de acceso https)
            services.AddCors(opt => {
                opt.AddPolicy("CorsPolicy", policy => {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                });
            });


        }




        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

     

            if (env.IsDevelopment())
            {
            }


            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions{
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Content")
                ), RequestPath = "/content"
            });

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwaggerDocumentation();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}
