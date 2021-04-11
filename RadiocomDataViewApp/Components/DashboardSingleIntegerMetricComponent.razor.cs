using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blazorise.Charts;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.CSharp;
using Microsoft.AspNetCore.Components.Web;
using Blazorise;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using RadiocomDataViewApp.Pages;
using RadiocomDataViewApp.Objects;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Clients;

namespace RadiocomDataViewApp.Components
{
    public partial class DashboardSingleIntegerMetricComponent : ComponentBase
    {
        private int _integerMetricValue;
        

        [Inject]
        public IRadiocomDataAggregateDataClient RadiocomDataAggregateDataClient { get; set; }

        
        [Parameter]
        public string ChartTitle { get; set; }
        [Parameter]
        public Func<Task<int>> GenerateSingleDataMetric { get; set; }

        public async Task RefreshChartData()
        {
            _integerMetricValue = await (GenerateSingleDataMetric).Invoke();
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            await RefreshChartData();
        }
        //protected override void OnAfterRender(bool firstRender)
        //{
        //    base.OnAfterRender(firstRender);
        //    if (firstRender)
        //    {
        //        RefreshChartData();
        //        StateHasChanged();
        //    }
        //}
    }
}
