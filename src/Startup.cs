using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using Telegram.Bot;
using TelegrammBott.Commands;

namespace TelegrammBott
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
            var botAccessToken = Configuration.GetSection("BotAccessToken");
            services.AddControllersWithViews();
            services.AddHostedService<BotService>();
            services.AddSingleton(new TelegramBotClient(botAccessToken.Value));
            services.AddSingleton<ICommand, HelloCommand>();
            services.AddSingleton<ICommand, DownloadImageCommand>();
            services.AddSingleton<ICommand, GetImageListCommand>();
            services.AddSingleton<ICommand, RotateImageCommand>();
            services.AddSingleton<ICommand, HelpCommand>();
            services.AddSingleton(new MessageInfoHub());
            services.AddSignalR();
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
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<MessageInfoHub>("/messageHub");
            });
        }
    }
}
