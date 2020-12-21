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

namespace RadiocomDataViewApp
{
    public class Program
    {
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
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


            var host = builder.Build();
            //host.Services
            //    .UseBootstrapProviders()
            //    .UseFontAwesomeIcons();
                
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
