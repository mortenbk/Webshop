using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Google.Apis.Gmail.v1;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebShop.RabbitMQ;
using RabbitMQ.Client;
using Autofac;
using Gmail.API.Events;
using Gmail.API.Events.Handlers;

namespace Gmail.API
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

            //services.AddIdentity<Google.Apis.Auth.OAuth2., IdentityRole>()
            //    .AddDefaultTokenProviders();

            //services.AddAuthentication(options =>
            //    {
            //        //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //        //options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; 
            //        //JwtBearerDefaults.AuthenticationScheme;
            //        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            //        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            //    }
            //)
            //    .AddGoogle(options =>
            //    {

            //        IConfigurationSection googleAuthNSection =
            //            Configuration.GetSection("Authentication:Google");
            //        options.Scope.Add(GmailService.Scope.GmailSend);
            //        options.ClientId = googleAuthNSection["ClientId"];
            //        options.ClientSecret = googleAuthNSection["ClientSecret"];
            //        options.SaveTokens = true;
            //        //options.SignInScheme = IdentityConstants.ExternalScheme;
            //    }).AddCookie(JwtBearerDefaults.AuthenticationScheme);

            services.AddControllers();

            AddRabbitMQ(services);
        }

        public IServiceCollection AddRabbitMQ(IServiceCollection services)
        {
            services.AddSingleton<IRabbitMQConnection>(sp =>
            {
                //var settings = sp.GetRequiredService<IOptions<CatalogSettings>>().Value;
                var logger = sp.GetRequiredService<ILogger<RabbitMQConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["RabbitMQConnection"],
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(Configuration["RabbitMQUserName"]))
                {
                    factory.UserName = Configuration["RabbitMQUserName"];
                }

                if (!string.IsNullOrEmpty(Configuration["RabbitMQPassword"]))
                {
                    factory.Password = Configuration["RabbitMQPassword"];
                }

                return new RabbitMQConnection(factory, logger, retryCount: 5);
            });

            services.AddSingleton<IEventBus, RabbitMQConnectionService>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<RabbitMQConnectionService>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<ISubscriptionManager>();


                return new RabbitMQConnectionService(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, "Email", retryCount: 5);
            });
            services.AddSingleton<ISubscriptionManager, SubscriptionManager>();
            services.AddTransient<SendEmailRequestEventHandler>();
            return services;
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac, like:
            builder.RegisterModule(new GmailModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            //app.UseAuthentication();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            ConfigureEventBus(app);
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<SendEmailRequestMessageEvent, SendEmailRequestEventHandler>();
        }
    }
}
