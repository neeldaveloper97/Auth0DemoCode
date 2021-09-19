using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleLogin.Database;
using SimpleLogin.Shared;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLogin.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public delegate ISender ServiceResolver(string key);

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<Context>(c => c.UseSqlServer(Configuration.GetConnectionString("DefaultDatabase")));

            services.AddDbContext<Context>(
    options =>
        options.UseSqlServer(
            Configuration.GetConnectionString("DefaultDatabase"),
            x => x.MigrationsAssembly("SimpleLogin.Database")));

            services.AddTransient<EmailSender>();
            services.AddTransient<DebugOutputSender>();

            services.AddTransient<ServiceResolver>(serviceProvider => key =>
            {
                switch (key)
                {
                    case "Email":
                        return serviceProvider.GetService<EmailSender>();
                    case "DebugOutput":
                        return serviceProvider.GetService<DebugOutputSender>();
                    default:
                        throw new KeyNotFoundException(); // or maybe return null, up to you
                }
            });

            //services.AddScoped<ISender, EmailSender>();
            services.Configure<Auth0>(Configuration.GetSection("Auth0"));
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
