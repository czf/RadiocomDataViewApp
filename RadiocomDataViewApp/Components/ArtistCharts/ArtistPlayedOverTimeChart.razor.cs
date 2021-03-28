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
    public partial class ArtistPlayedOverTimeChart : ComponentBase
    {
        private DashboardLineGraphChartComponent Chart;
        private List<HeaderButtonState> HeaderButtonConfigs { get; set; }
        private AggregateTimeRange ChartDataTimeRange = AggregateTimeRange.SevenDays;

        [Parameter]
        public int ArtistId { get; set; }

        [Inject]
        public IRadiocomDataAggregateDataClient RadiocomDataAggregateDataClient { get; set; }
        public ArtistPlayedOverTimeChart()
        {
            HeaderButtonConfigs = new List<HeaderButtonState>()
            {
                 new HeaderButtonState(){Text = "7 Days",ButtonColor=Color.Secondary,Active=true, ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.SevenDays)) } ,
                new HeaderButtonState(){Text = "3 Months", ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.ThreeMonths)) } ,
                new HeaderButtonState(){Text = "One Year", ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.OneYear)) }
            };

            ChartDataTimeRange = AggregateTimeRange.SevenDays;
        }


        private async Task UpdateChartDataTimeRange(AggregateTimeRange mostPlayedTimeRange)
        {
            ChartDataTimeRange = mostPlayedTimeRange;
            await Chart.RefreshChartData();
        }
        private  async Task<IEnumerable<DashboardChartData>> ArtistPlayedOverTime()
        {
            List<ItemCount> radioComData = await RadiocomDataAggregateDataClient.GetArtistPlayedOverTime(ChartDataTimeRange, ArtistId);
            return radioComData.Select(x => new DashboardChartData() { Label = x.Name, Value = x.Count, DataId = x.ItemId });
        }
    }
}
