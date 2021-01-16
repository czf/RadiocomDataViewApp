using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using RadiocomDataViewApp.Clients;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects;

namespace RadiocomDataViewApp.Components.ArtistCharts
{
    public partial class TopPlayedArtistSongsChart : ComponentBase
    {
        private List<HeaderButtonState> HeaderButtonConfigs;
        private AggregateTimeRange ChartDataTimeRange;
        private DashboardHorizontalBarChartComponent Chart;
        
        [Parameter]
        public int ArtistId { get; set; }
        
        [Inject]
        public NavigationManager NavManager { get; set; }
        [Inject]
        public IRadiocomDataAggregateDataClient RadiocomDataAggregateDataClient { get; set; }



        public TopPlayedArtistSongsChart()
        {
            HeaderButtonConfigs = new List<HeaderButtonState>()
            {
                 new HeaderButtonState(){Text = "7 Days",ButtonColor=Color.Secondary,Active=true, ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.SevenDays)) } ,
                new HeaderButtonState(){Text = "3 Months", ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.ThreeMonths)) } ,
                new HeaderButtonState(){Text = "All Time", ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.AllTime)) }
            };
            ChartDataTimeRange = AggregateTimeRange.SevenDays;

        }


        private void NavigateToArtistArtistsWorkRouteOnBarClick(DashboardChartMouseEventArgs args)
        {
            BarChartDatasetXValue element = (BarChartDatasetXValue)args.DatasetElement;
            NavManager.NavigateTo($"artist/{element.DataId}/artistworks");
        }

        private async Task UpdateChartDataTimeRange(AggregateTimeRange mostPlayedTimeRange)
        {
            ChartDataTimeRange = mostPlayedTimeRange;
            await Chart.RefreshChartData();
        }

        private IEnumerable<DashboardChartData> TopPlayedArtistSongs()
        {
            List<ItemCount> radioComData = RadiocomDataAggregateDataClient.GetMostPlayedSongs(ChartDataTimeRange, ArtistId);
            return radioComData.Select(x => new DashboardChartData() { Label = x.Name, Value = x.Count, DataId = x.ItemId });
        }
    }
}
