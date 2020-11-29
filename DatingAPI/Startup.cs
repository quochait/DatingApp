using System.Net;
using System.Text;
using DatingAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using DatingAPI.Helpers;
using DatingAPI.MongoHelper;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DatingAPI.Services.Photo;
using DatingAPI.Hubs;
using DatingAPI.Services.Group;
using DatingAPI.Services.Message;
using Microsoft.AspNet.SignalR;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;

namespace DatingAPI
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
      //services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
      services.AddCors();
      services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
          .AddJsonOptions(opt =>
          {
            opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //opt.se
          });
      //services.AddMvcCore().AddNewtonsoftJson();
      services.AddScoped<IAuthenticationServices, AuthenticationServices>();
      services.AddScoped<IUserServices, UserServices>();
      services.AddScoped<IPhotoServices, PhotoServices>();
      services.AddScoped<IGroupServices, GroupServices>();
      services.AddScoped<IMessageServices, MessageServices>();

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
          AddJwtBearer(options =>
          {
            options.TokenValidationParameters = new TokenValidationParameters
            {
              ValidateIssuerSigningKey = true,
              IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
              ValidateIssuer = false,
              ValidateAudience = false
            };
            options.Events = new JwtBearerEvents()
            {
              OnMessageReceived = context =>
              {
                if (context.Request.Query.TryGetValue("token", out StringValues token))
                {
                  context.Token = token;
                }
                return Task.CompletedTask;
              }
            };
          })
          ;

      services.AddScoped<LogUserActivity>();
      services.AddTransient<IMongoContext, MongoContext>();
      services.AddAutoMapper();
      services.AddMvc();
      services.AddMemoryCache();
      services.AddSignalR();
      
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseExceptionHandler(builder =>
        {
          builder.Run(async context =>
                  {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    var error = context.Features.Get<IExceptionHandlerFeature>();

                    if (error != null)
                    {
                      context.Response.AddApplicationError(error.Error.Message);
                      await context.Response.WriteAsync(error.Error.Message);
                    }
                  });
        });
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app
        .UseCors(
        x =>
        x
        .WithOrigins(
        "http://localhost:4200",
        "http://192.168.1.148:4200",
        "http://192.168.43.109:4200",
        "https://192.168.43.109:4200",
        "http://172.16.171.100:4200",
        "https://172.16.171.100:4200"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());

      app.UseAuthentication();
      app.UseMvc();
      app.UseSignalR(routes =>
      {
        routes.MapHub<SignalrHub>("/chathub", map =>
        {
          //var hubConfiguration = new HubConfiguration();

        });
      });
    }
  }
}
