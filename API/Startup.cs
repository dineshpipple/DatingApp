using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using API.Interface;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Extensions;
using API.MiddleWare;

namespace API {
  public class Startup {
    private readonly IConfiguration _config;
    public Startup(IConfiguration config) {
      _config = config;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {

      services.AddApplicationService(_config);
      services.AddControllers();
      services.AddSwaggerGen(c => {
        c.SwaggerDoc("v1", new OpenApiInfo {
          Title = "API",
          Version = "v1"
        });
      });
      services.AddCors();
      services.AddIdentityService(_config);
      
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
     
      app.UseMiddleware<ExceptionMiddleWare>();
      
      if (env.IsDevelopment()) {
        // app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
      }

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthentication();

      app.UseAuthorization();

      app.UseCors(x=> x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

      app.UseEndpoints(endpoints => {
        endpoints.MapControllers();
      });
    }
  }
}
