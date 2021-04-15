using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityGuide.Api.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CityGuide.Api.Helpers;

namespace CityGuide.Api
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
            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value);

            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddControllers();
            services.AddDbContext<DataContext>(x => x.UseSqlServer("Server=desktop-psjtqan;database=CityGuide;integrated security=true"));
            services.AddScoped<IAppRepository, AppRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddAutoMapper(typeof(Startup));
            services.AddCors();
            services.AddMvc();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options=>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey=true,
                    IssuerSigningKey=new SymmetricSecurityKey(key),
                    ValidateIssuer=false,
                    ValidateAudience=false
                };
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // app.UseCors(x=>x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());

            app.UseAuthorization();

            app.UseAuthentication();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
