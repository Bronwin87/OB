using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PayFast;
using Shop.Application.UsersAdmin;
using Shop.Database;
using Shop.Domain.Models;
using Shop.Domain.Models.Users;
using Shop.UI.Utilities;
using System;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using Abiosoft.DotNet.DevReload;
using Cin7ApiWrapper.Infrastructure;
using Cin7ApiWrapper.Common;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;

namespace Shop.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            //_config = configuration;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            _config = builder.Build();
        }

        public IConfiguration _config { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    options.CheckConsentNeeded = context => false;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});
            //string originDomain = Context.Request.Host.Value;

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(_config["DefaultConnection"]));
            services.AddMemoryCache();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.SignIn.RequireConfirmedEmail = false;
                //options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Accounts/Login";
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireClaim("Role", "Admin"));
                //options.AddPolicy("Manager", policy => policy.RequireClaim("Role", "Manager"));
                options.AddPolicy("Manager", policy => policy
                    .RequireAssertion(context =>
                        context.User.HasClaim("Role", "Manager")
                        || context.User.HasClaim("Role", "Admin")));
            });

            services
                .AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Admin");
                    options.Conventions.AuthorizePage("/Admin/ConfigureUsers", "Admin");
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSession(options =>
            {
                options.Cookie.Name = "Cart";
                options.Cookie.MaxAge = TimeSpan.FromDays(355);
            });

            services.AddTransient<CreateUser>();
            Action<Discounts> discounts = (opt =>
            {

            });
            services.Configure(discounts);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<Discounts>>().Value);

            services.AddDetection();
            services.Configure<PayFastSettings>(_config.GetSection("PayFastSettings"));
            services.Configure<Cin7ApiWrapper.Common.Cin7Api>(_config.GetSection("Cin7ApiSettings"));

            // services.Configure<Cin7ApiWrapper.Models.>(_config.GetSection("Cin7ApiBaseUrl"));
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddScoped<CustomClaimsCookieSignInHelper<ApplicationUser>>();
            services.AddScoped<CookiesHelper>();
            services.RegisterApplicationServices();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto;
                // Only loopback proxies are allowed by default.
                // Clear that restriction because forwarders are enabled by explicit 
                // configuration.
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            services.AddHttpsRedirection(o =>
            {
                o.HttpsPort = 443;
                //o.RedirectStatusCode = StatusCodes.
            });

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // In the Configure function in Startup.cs

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDevReload();
            }
            else
            {
                app.UseForwardedHeaders();
                loggerFactory.AddAWSProvider(_config.GetAWSLoggingConfigSection());
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                // HTTPS redirect only on live site
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseSession();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
