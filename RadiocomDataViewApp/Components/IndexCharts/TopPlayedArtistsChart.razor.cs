using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using RadiocomDataViewApp.Clients;
using RadiocomDataViewApp.Objects;

namespace RadiocomDataViewApp.Components.IndexCharts
{
    public partial class TopPlayedArtistsChart : ComponentBase
    {
        public TopPlayedArtistsChart()
        {
            HeaderButtonConfigs = new List<HeaderButtonState>()
            {
                new HeaderButtonState(){Text = "7 Days",ButtonColor=Color.Secondary,Active=true, ButtonClickCallback = EventCallback.Factory.Create(this, () => {ChartDataTimeRange = MostPlayedTimeRange.SevenDays; Chart.RefreshChartData();}) } ,
                new HeaderButtonState(){Text = "3 Months", ButtonClickCallback = EventCallback.Factory.Create(this, async() => {await Task.Delay(5000); ChartDataTimeRange =MostPlayedTimeRange.ThreeMonths; Chart.RefreshChartData();}) } ,
                new HeaderButtonState(){Text = "All Time", ButtonClickCallback = EventCallback.Factory.Create(this, () => {ChartDataTimeRange =MostPlayedTimeRange.AllTime; Chart.RefreshChartData();}) }
            };

            ChartDataTimeRange = MostPlayedTimeRange.SevenDays;
        }

        private List<HeaderButtonState> HeaderButtonConfigs;
        private MostPlayedTimeRange ChartDataTimeRange;
        private DashboardChartComponent Chart;
        private IEnumerable<DashboardChartData> TopPlayedArtists()
        {

            List<ItemCount> radioComData = RadiocomDataAggregateDataClient.GetMostPlayedArtists(ChartDataTimeRange);
            return radioComData.Select(x => new DashboardChartData() { Label = x.Name, Value = x.Count, DataId = x.ItemId });
        }

        private void NavigateToArtistRouteOnBarClick(DashboardChartMouseEventArgs args)
        {
            BarChartDatasetXValue element = (BarChartDatasetXValue)args.DatasetElement;
            NavManager.NavigateTo($"artistwork/{element.DataId}");
        }
    }
}