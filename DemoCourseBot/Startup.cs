// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using DemoCourseBot.Bots;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Builder.BotFramework;
using DemoCourseBot.Services;
using DemoCourseBot.Dialogs;

namespace DemoCourseBot
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();



            // Create the credential provider to be used with the Bot Framework Adapter.
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();


            // Create the Bot Framework Adapter.
            services.AddSingleton<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>();

            services.AddSingleton<BotServices>();

            //configure state

            ConfigureState(services);

            //dialogs

            ConfigureDialogs(services);

            //starting project
            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            //  services.AddTransient<IBot, EchoBot>();
            // services.AddTransient<IBot, GreetingBot>();
            //now top one changed to maindialog because of waterfall integartion

            services.AddTransient<IBot, DialogBot<MainDialog>>();

            //LUIS INtegration.
           // services.AddTransient<IBot, DialogBot<BotLuisDialog>>();
        }

        public void ConfigureState(IServiceCollection services)
        {
            //var storageAccount = "";
            //var storageContainer = "";
            //services.AddSingleton<IStorage>(new AzureBlobStorage(storageAccount, storageContainer));
            //created  a storage for storing local user state and conversation state (testing)

            services.AddSingleton<IStorage, MemoryStorage>();
            //userstate
            services.AddSingleton<UserState>();
            //conversationstate

            services.AddSingleton<ConversationState>();

            //insatnce of stateclass

            services.AddSingleton<BotStateService>();
        }

        public void ConfigureDialogs(IServiceCollection services)
        {
            services.AddSingleton<MainDialog>();

            //luis
           //services.AddSingleton<BotLuisDialog>();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWebSockets();
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
