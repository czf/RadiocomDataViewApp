using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Clients;
using RadiocomDataViewApp.Components.IndexCharts;
using RadiocomDataViewApp.Clients.Mocks;
using Blazored.LocalStorage;
using RadiocomDataViewApp.Services;

namespace RadiocomDataViewApp
{
    public class Program
    {
        private const string CONFIGKEY_WELCOME_DATE = "welcome";
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            builder.RootComponents.Add<App>("app");
            
            builder.Services.AddSingleton(sp => new EnvironmentService { IsDevelopment = builder.HostEnvironment.IsDevelopment() });
            builder.Services.AddSingleton<IRadiocomDataAggregateDataClient, MockRadiocomDataAggregateDataClient>();
            builder.Services.AddSingleton<IRadiocomArtistRepository, MockRadiocomArtistRepository>();
            builder.Services.AddSingleton<IRadiocomArtistWorkRepository, MockRadiocomArtistWorkRepository>();
            builder.Services.AddScoped<IVisitService, VisitService>(x=> {
                DateTime welcomeDate =  builder.Configuration.GetValue<DateTime>(CONFIGKEY_WELCOME_DATE);
                return new VisitService(x.GetService<ILocalStorageService>(), welcomeDate);            
            });
            builder.Services.AddScoped<IUpdateService>(x =>
            {
                DateTime welcomeDate = builder.Configuration.GetValue<DateTime>(CONFIGKEY_WELCOME_DATE);
                return new UpdateService(x.GetService<HttpClient>(), welcomeDate);
            });


            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddBlazoredLocalStorage();
            
            string value =string.Empty;
            value = (string)builder.Configuration.GetValue<string>("version");
            Console.WriteLine($"version: {value}");
            var host = builder.Build();
            host.Services
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();
                
            await host.RunAsync();
        }

        //public static temp()
        //{

        //    System.Security.Cryptography.SHA256Managed.
        //    System.Security.Cryptography.SHA256.

        //}
    }
    public class EnvironmentService
    {
        public bool IsDevelopment { get; set; }
    }
}
