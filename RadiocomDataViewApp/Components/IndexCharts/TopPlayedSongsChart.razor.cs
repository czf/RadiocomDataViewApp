﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using RadiocomDataViewApp.Clients;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects;

namespace RadiocomDataViewApp.Components.IndexCharts
{
    public partial class TopPlayedSongsChart : ComponentBase
    {
        private DashboardHorizontalBarChartComponent Chart;
        private AggregateTimeRange ChartDataTimeRange;

        private List<HeaderButtonState> HeaderButtonConfigs { get; set; }



        [Inject]
        public NavigationManager NavManager { get; set; }
        [Inject]
        public IRadiocomDataAggregateDataClient RadiocomDataAggregateDataClient { get; set; }


        public TopPlayedSongsChart()
        {
            HeaderButtonConfigs = new List<HeaderButtonState>()
            {
                new HeaderButtonState(){Text = "7 Days",ButtonColor=Color.Secondary,Active=true, ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.SevenDays)) } ,
                new HeaderButtonState(){Text = "3 Months", ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.ThreeMonths)) } ,
                new HeaderButtonState(){Text = "One Year", ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.OneYear)) } 
            };

            ChartDataTimeRange = AggregateTimeRange.SevenDays;
        }


        

        
        private async Task<IEnumerable<DashboardChartData>> TopPlayedSongs()
        {
            List<ItemCount> radioComData = await RadiocomDataAggregateDataClient.GetMostPlayedSongsAsync(ChartDataTimeRange);
            return radioComData.Select(x => new DashboardChartData() { Label = x.Name, Value = x.Count, DataId = x.ItemId });
        }
        
        private void NavigateToSongRouteOnBarClick(DashboardChartMouseEventArgs args)
        {
            BarChartDatasetXValue element = (BarChartDatasetXValue)args.DatasetElement;
            NavManager.NavigateTo($"artistwork/{element.DataId}");
        }

        private async Task UpdateChartDataTimeRange(AggregateTimeRange mostPlayedTimeRange)
        {
            ChartDataTimeRange = mostPlayedTimeRange;
            await Chart.RefreshChartData();
        }

    }
}