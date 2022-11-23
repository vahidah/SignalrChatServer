using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalRBackEnd.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRBackEnd
{
    public class Startup
    {
        public async Task<string> SendMessage(Message message)
        {
            return await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("D:/projects/django/fcm/serviceAccountKey.json"),
            });


            //var message = new Message()
            //{
            //    Notification = new Notification()
            //    {
            //        Title = "new notif",
            //        Body = "body of notif"

            //    },

            //    Token = "cA3iQOv8TDyU-9gfbf4_gE:APA91bFvjPdHsXFWANTcpvHHHGifOK5az0MiPQKHvRb8hLeq2r7wvS5zKMdbGWowOGuJT-852uiBMOSFBKRt0F1y0IIUYx_aap71B2z_PJ3Zf9E5-WwfzPiK-EVDwwjLMe93y2fX1C5Z",
            //};


            //// Response is a message ID string.
            //Console.WriteLine("Successfully sent message: " + SendMessage(message));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
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
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MyHub>("/Myhub");
            });
        }
    }
}
