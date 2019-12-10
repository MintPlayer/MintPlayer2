using System;
using System.Linq;
using AspNetCoreOpenSearch.Extensions;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MintPlayer.Data.Extensions;
using MintPlayer.Data.Options;
using SitemapXml;
using Spa.SpaRoutes;

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
            services
                .AddMintPlayer(options => {
                    options.ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=MintPlayer;Trusted_Connection=True;ConnectRetryCount=0";
                    options.JwtIssuerOptions = new Data.Options.JwtIssuerOptions
                    {
                        Issuer = Configuration["JwtIssuerOptions:Issuer"],
                        Audience = Configuration["JwtIssuerOptions:Audience"],
                        Subject = Configuration["JwtIssuerOptions:Subject"],
                        ValidFor = Configuration.GetValue<TimeSpan>("JwtIssuerOptions:ValidFor"),
                        Key = Configuration["JwtIssuerOptions:Key"],
                    };
                    options.FacebookOptions = new FacebookOptions
                    {
                        AppId = Configuration["FacebookOptions:AppId"],
                        AppSecret = Configuration["FacebookOptions:AppSecret"]
                    };
                    options.MicrosoftOptions = new MicrosoftAccountOptions
                    {
                        ClientId = Configuration["MicrosoftOptions:AppId"],
                        ClientSecret = Configuration["MicrosoftOptions:AppSecret"]
                    };
                    options.GoogleOptions = new GoogleOptions
                    {
                        ClientId = Configuration["GoogleOptions:AppId"],
                        ClientSecret = Configuration["GoogleOptions:AppSecret"]
                    };
                    options.TwitterOptions = new TwitterOptions
                    {
                        ConsumerKey = Configuration["TwitterOptions:ApiKey"],
                        ConsumerSecret = Configuration["TwitterOptions:ApiSecret"]
                    };
                })
                .AddSitemapXml()
                .AddElasticSearch(options => {
                    options.Url = Configuration["elasticsearch:url"];
                    options.DefaultIndex = Configuration["elasticsearch:index"];
                })
                .AddControllersWithViews(options =>
                {
                    options.RespectBrowserAcceptHeader = true;
                })
                .AddXmlSerializerFormatters()
                .AddSitemapXmlFormatters(options => {
                    options.StylesheetUrl = "/assets/sitemap.xsl";
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                })
                .AddNewtonsoftJson();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddOpenSearch<Services.OpenSearchService>();
            
            services.AddSpaRoutes(routes => routes
                .Route("", "home")
                .Group("search", "search", search_routes => search_routes
                    .Route("", "search")
                    .Route("{term}", "results")
                )
                .Group("person", "person", person_routes => person_routes
                    .Route("", "list")
                    .Route("create", "create")
                    .Route("{id}", "show")
                    .Route("{id}/edit", "edit")
                )
                .Group("artist", "artist", artist_routes => artist_routes
                     .Route("", "list")
                     .Route("create", "create")
                     .Route("{id}", "show")
                     .Route("{id}/edit", "edit")
                )
                .Group("song", "artist", song_routes => song_routes
                     .Route("", "list")
                     .Route("create", "create")
                     .Route("{id}", "show")
                     .Route("{id}/edit", "edit")
                )
                .Group("medium-type", "mediumtype", mediumtype_routes => mediumtype_routes
                     .Route("", "list")
                     .Route("create", "create")
                     .Route("{id}", "show")
                     .Route("{id}/edit", "edit")
                )
                .Group("account", "account", account_routes => account_routes
                    .Route("login", "login")
                    .Route("register", "register")
                    .Route("profile", "profile")
                )
            );

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

                }).Configure<RazorViewEngineOptions>(options => {
                    var new_locations = options.ViewLocationFormats.Select(vlf => $"/Server{vlf}").ToList();
                    options.ViewLocationFormats.Clear();
                    foreach (var format in new_locations)
                        options.ViewLocationFormats.Add(format);
                })
                .Configure<JwtIssuerOptions>(options =>
                {
                    options.Issuer = Configuration["JwtIssuerOptions:Issuer"];
                    options.Subject = Configuration["JwtIssuerOptions:Subject"];
                    options.Audience = Configuration["JwtIssuerOptions:Audience"];
                    options.ValidFor = Configuration.GetValue<TimeSpan>("JwtIssuerOptions:ValidFor");
                    options.Key = Configuration["JwtIssuerOptions:Key"];
                })
                .ConfigureApplicationCookie(options =>
                {
                    options.Events.OnRedirectToLogin = async (context) =>
                    {
                        context.Response.StatusCode = 401;
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
                .UseOpenSearch(options =>
                {
                    options.OsdxEndpoint = "/opensearch.xml";
                    options.SearchUrl = "/web/OpenSearch/redirect/{searchTerms}";
                    options.SuggestUrl = "/web/OpenSearch/suggest/{searchTerms}";
                    options.ImageUrl = "/assets/logo/music_note_16.png";
                    options.ShortName = "MintPlayer";
                    options.Description = "Search music on MintPlayer";
                    options.Contact = "pieterjandeclippel@msn.com";
                })
                .UseDefaultSitemapXmlStylesheet(options =>
                {
                    options.StylesheetUrl = "/assets/sitemap.xsl";
                })
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

                    spa.UseSpaPrerendering(options => {
                        options.BootModulePath = $"{spa.Options.SourcePath}/dist/server/main.js";
                        options.BootModuleBuilder = env.IsDevelopment()
                            ? new AngularCliBuilder(npmScript: "build:ssr")
                            : null;
                        options.ExcludeUrls = new[] { "/sockjs-node" };
                    });

                    if (env.IsDevelopment())
                    {
                        spa.UseAngularCliServer(npmScript: "start");
                    }
                });
        }
    }
}
