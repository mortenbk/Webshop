using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using Cart.API.Events;
using Cart.API.Events.Handlers;
using Cart.API.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using WebShop.RabbitMQ;

namespace Cart.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            AddRabbitMQ(services);


            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Cart API - Cart HTTP API",
                    Version = "v1",
                    Description = "The Cart Service HTTP API"
                });
            });

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<CartContext>(options =>
                {
                    options.UseSqlServer(Configuration["ConnectionString"],
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });
                },
                    ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                );




        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac, like:
            builder.RegisterModule(new CartModule());
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


                return new RabbitMQConnectionService(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, "Cart", retryCount: 5);
            });
            services.AddSingleton<ISubscriptionManager, SubscriptionManager>();
            services.AddTransient<ProductPriceChangedEventHandler>();
            services.AddTransient<ProductNameChangedEventHandler>();
            services.AddTransient<ProductRemovedEventHandler>();

            return services;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseRouting();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cart API V1");
                c.RoutePrefix = String.Empty;
            });



            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Migrates db if needed
            var context = app.ApplicationServices.GetService<CartContext>();
            context.Database.Migrate();

            ConfigureEventBus(app);
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<ProductPriceChangedMessageEvent, ProductPriceChangedEventHandler>();
            eventBus.Subscribe<ProductNameChangedMessageEvent, ProductNameChangedEventHandler>();
            eventBus.Subscribe<ProductRemovedMessageEvent, ProductRemovedEventHandler>();
        }
    }
}
