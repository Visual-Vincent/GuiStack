using System;
using System.IO;
using System.Linq;
using Amazon;
using GuiStack.Repositories;
using GuiStack.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GuiStack
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
            services.AddRazorPages();
            services.AddControllers();
            services.AddSession();
            services.AddDistributedMemoryCache();
            services.AddScoped<IS3Repository, S3Repository>();
            services.AddScoped<ISQSRepository, SQSRepository>();
            services.AddSingleton<IS3UrlBuilder, S3UrlBuilder>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            var contentTypeProvider = new FileExtensionContentTypeProvider();

            if(!contentTypeProvider.Mappings.ContainsKey(".apng"))
                contentTypeProvider.Mappings.Add(".apng", "image/apng");

            app.UseStaticFiles(new StaticFileOptions() {
                ContentTypeProvider = contentTypeProvider
            });

            app.UseRouting();
            app.UseAuthorization();

            app.UseSession(new SessionOptions() {
                IdleTimeout = TimeSpan.FromHours(2)
            });

            app.UseEndpoints(endpoints => {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });

            AWSConfigs.AWSRegion = Environment.GetEnvironmentVariable("AWS_REGION") ?? "eu-central-1";

            EnumerationOptions enumerationOptions = new EnumerationOptions() {
                IgnoreInaccessible = true,
                RecurseSubdirectories = false,
                ReturnSpecialDirectories = false
            };

            // Delete old temp files
            foreach(string dir in Directory.EnumerateDirectories(Path.GetTempPath(), "guistack-proto-*", enumerationOptions))
            {
                if(Path.GetFileName(dir).Count(c => c == '-') > 2)
                    continue;

                try { Directory.Delete(dir, true); } catch { }
            }
        }
    }
}
