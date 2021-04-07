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
using RadiocomDataViewApp.Clients.Live;

namespace RadiocomDataViewApp
{
    public class Program
    {
        private const string CONFIGKEY_WELCOME_DATE = "welcome";
        private const string CONFIGKEY_USE_MOCKS = "useMocks";
        private const string CONFIGKEY_ENDPOINT_ADDRESS = "endpointAddress";

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
            builder.Services.AddScoped<IRadiocomArtistRepository>(x=>
            {
                IRadiocomArtistRepository service;
                bool useMocksFlag = builder.Configuration.GetValue<bool>(CONFIGKEY_USE_MOCKS);
                if (useMocksFlag)
                {
                    service = new MockRadiocomArtistRepository();
                }
                else
                {
                    string endpointAddress = builder.Configuration.GetValue<string>(CONFIGKEY_ENDPOINT_ADDRESS);
                    service = new LiveRadiocomArtistRepository(x.GetService<HttpClient>(), x.GetService<ILocalStorageService>(), endpointAddress);
                }
                return service;
            });

            builder.Services.AddScoped<IRadiocomArtistWorkRepository>(x =>
            {
                IRadiocomArtistWorkRepository service;
                bool useMocksFlag = builder.Configuration.GetValue<bool>(CONFIGKEY_USE_MOCKS);
                if (useMocksFlag)
                {
                    service = new MockRadiocomArtistWorkRepository(x.GetService<IRadiocomArtistRepository>());
                }
                else
                {
                    string endpointAddress = builder.Configuration.GetValue<string>(CONFIGKEY_ENDPOINT_ADDRESS);
                    service = new LiveRadiocomArtistWorkRepository(x.GetService<IRadiocomArtistRepository>(), x.GetService<HttpClient>(), x.GetService<ILocalStorageService>(), endpointAddress);
                }
                return service;
            });


            builder.Services.AddScoped<IRadiocomDataAggregateDataClient>(x=>
            {
                IRadiocomDataAggregateDataClient service;
                bool useMocksFlag = builder.Configuration.GetValue<bool>(CONFIGKEY_USE_MOCKS);
                if (useMocksFlag)
                {
                    service = new MockRadiocomDataAggregateDataClient();
                }
                else
                {
                    string endpointAddress = builder.Configuration.GetValue<string>(CONFIGKEY_ENDPOINT_ADDRESS);
                    service = new LiveRadiocomDataAggregateDataClient(x.GetService<HttpClient>(), x.GetService<ILocalStorageService>(), endpointAddress);
                }
                return service;
            });


            builder.Services.AddBlazoredLocalStorage();
            
            Console.WriteLine($"WelcomeDate: {builder.Configuration.GetValue<DateTime>(CONFIGKEY_WELCOME_DATE)}");
            await builder.Build().RunAsync();

        }


    }
    public class EnvironmentService
    {
        public bool IsDevelopment { get; set; }
    }
}
