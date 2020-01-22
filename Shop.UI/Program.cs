using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Shop.Database;
using Shop.Domain.Models.Users;
using System;
using System.Linq;
using System.Security.Claims;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Logging;

namespace Shop.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.Debug()
            //    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            //    .Enrich.FromLogContext()
            //    .WriteTo.Console()
            //    .CreateLogger();

            try
            {
                //Log.Information("Starting web host");
                var host = CreateWebHostBuilder(args).Build();
                try
                {
                    using (var scope = host.Services.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var userManger = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                        context.Database.EnsureCreated();
                        if (!context.Users.Any())
                        {
                            var adminUser = new ApplicationUser()
                            {
                                UserName = "admin@test.com",
                                Email = "admin@test.com"
                            };
                            userManger.CreateAsync(adminUser, "password").GetAwaiter().GetResult();
                            var adminClaim = new Claim("Role", "Admin");
                            userManger.AddClaimAsync(adminUser, adminClaim).GetAwaiter().GetResult();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host Termintated unexpectedly");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                // https://github.com/aws/aws-logging-dotnet
                //.ConfigureLogging(logging =>
                //{
                //    logging.AddAWSProvider();

                //    // When you need logging below set the minimum level. 
                //    // Otherwise the logging framework will default to Informational for external providers.
                //    logging.SetMinimumLevel(LogLevel.Debug);
                //})
                .UseStartup<Startup>();
                //.UseSerilog();
    }
}
