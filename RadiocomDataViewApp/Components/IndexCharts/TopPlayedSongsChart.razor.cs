using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using RadiocomDataViewApp.Clients;
using RadiocomDataViewApp.Objects;

namespace RadiocomDataViewApp.Components.IndexCharts
{
    public partial class TopPlayedSongsChart : ComponentBase
    {
        public TopPlayedSongsChart()
        {
            HeaderButtonConfigs = new List<HeaderButtonState>()
            {
                new HeaderButtonState(){Text = "7 Days",ButtonColor=Color.Secondary,Active=true, ButtonClickCallback = EventCallback.Factory.Create(this, () => {ChartDataTimeRange = MostPlayedTimeRange.SevenDays; Chart.RefreshChartData();}) } ,
                new HeaderButtonState(){Text = "3 Months", ButtonClickCallback = EventCallback.Factory.Create(this, async() => {await Task.Delay(5000); ChartDataTimeRange =MostPlayedTimeRange.ThreeMonths; Chart.RefreshChartData();}) } ,
                new HeaderButtonState(){Text = "All Time", ButtonClickCallback = EventCallback.Factory.Create(this, () => {ChartDataTimeRange =MostPlayedTimeRange.AllTime; Chart.RefreshChartData();}) } 
            };

            ChartDataTimeRange = MostPlayedTimeRange.SevenDays;
        }

        public List<HeaderButtonState> HeaderButtonConfigs { get; set; }

        DashboardChartComponent Chart;
        private MostPlayedTimeRange ChartDataTimeRange;
        private IEnumerable<DashboardChartData> TopPlayedSongs()
        {
            
            List<ItemCount> radioComData = RadiocomDataAggregateDataClient.GetMostPlayedSongs(ChartDataTimeRange);
            return radioComData.Select(x => new DashboardChartData() { Label = x.Name, Value = x.Count, DataId = x.ItemId });
        }

        private void NavigateToSongRouteOnBarClick(DashboardChartMouseEventArgs args)
        {
            BarChartDatasetXValue element = (BarChartDatasetXValue)args.DatasetElement;
            NavManager.NavigateTo($"artistwork/{element.DataId}");
        }
    }
}