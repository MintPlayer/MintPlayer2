using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MintPlayer.Data.Extensions;
using MintPlayer.Data.Options;

namespace MintPlayer.Web
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
            services.AddMintPlayer(options => {
                options.ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=MintPlayer;Trusted_Connection=True;ConnectRetryCount=0";
                options.JwtIssuerOptions = new Data.Options.JwtIssuerOptions
                {
                    Issuer = Configuration["JwtIssuerOptions:Issuer"],
                    Audience = Configuration["JwtIssuerOptions:Audience"],
                    Subject = Configuration["JwtIssuerOptions:Subject"],
                    ValidFor = Configuration.GetValue<TimeSpan>("JwtIssuerOptions:ValidFor"),
                    Key = Configuration["JwtIssuerOptions:Key"],
                };
            });

            services
                .AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services
                .Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
                })
                .Configure<IdentityOptions>(options =>
                {
                    // Password settings
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = false;
                    options.Password.RequiredUniqueChars = 6;

                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = System.TimeSpan.FromMinutes(30);
                    options.Lockout.MaxFailedAccessAttempts = 10;
                    options.Lockout.AllowedForNewUsers = true;

                    // User settings
                    options.User.RequireUniqueEmail = true;
                    options.User.AllowedUserNameCharacters = string.Empty;

                })
                .Configure<JwtIssuerOptions>(options =>
                {
                    options.Issuer = Configuration["JwtIssuerOptions:Issuer"];
                    options.Subject = Configuration["JwtIssuerOptions:Subject"];
                    options.Audience = Configuration["JwtIssuerOptions:Audience"];
                    options.ValidFor = Configuration.GetValue<TimeSpan>("JwtIssuerOptions:ValidFor");
                    options.Key = Configuration["JwtIssuerOptions:Key"];
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app
                .UseHsts()
                .UseHttpsRedirection()
                .UseForwardedHeaders()
                .UseStaticFiles();

            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app
                .UseAuthentication()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller}/{action=Index}/{id?}");
                })
                .UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ClientApp";
                    if (env.IsDevelopment())
                    {
                        spa.UseAngularCliServer(npmScript: "start");
                    }
                });
        }
    }
}
