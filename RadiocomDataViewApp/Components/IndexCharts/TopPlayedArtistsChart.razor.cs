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
    public partial class TopPlayedArtistsChart : ComponentBase
    {
        [Inject]
        public NavigationManager NavManager { get; set; }
        [Inject]
        public IRadiocomDataAggregateDataClient RadiocomDataAggregateDataClient { get; set; }

        public TopPlayedArtistsChart()
        {
            HeaderButtonConfigs = new List<HeaderButtonState>()
            {
                 new HeaderButtonState(){Text = "7 Days",ButtonColor=Color.Secondary,Active=true, ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.SevenDays)) } ,
                new HeaderButtonState(){Text = "3 Months", ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.ThreeMonths)) } ,
                new HeaderButtonState(){Text = "One Year", ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.OneYear)) }
            };

            ChartDataTimeRange = AggregateTimeRange.SevenDays;
        }

        private List<HeaderButtonState> HeaderButtonConfigs;
        private AggregateTimeRange ChartDataTimeRange;
        private DashboardHorizontalBarChartComponent Chart;
        private async Task<IEnumerable<DashboardChartData>> TopPlayedArtists()
        {
            List<ItemCount> radioComData = await RadiocomDataAggregateDataClient.GetMostPlayedArtistsAsync(ChartDataTimeRange);
            return radioComData.Select(x => new DashboardChartData() { Label = x.Name, Value = x.Count, DataId = x.ItemId });
        }

        private void NavigateToArtistRouteOnBarClick(DashboardChartMouseEventArgs args)
        {
            BarChartDatasetXValue element = (BarChartDatasetXValue)args.DatasetElement;
            NavManager.NavigateTo($"artist/{element.DataId}");
        }
        private async Task UpdateChartDataTimeRange(AggregateTimeRange mostPlayedTimeRange)
        {
            ChartDataTimeRange = mostPlayedTimeRange;
            await Chart.RefreshChartData();
        }
    }
}