using System;
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
    public partial class TotalUniqueSongsMetric : ComponentBase
    {
        [Inject]
        public IRadiocomDataAggregateDataClient RadiocomDataAggregateDataClient { get; set; }
        private List<HeaderButtonState> HeaderButtonConfigs { get; }
        private AggregateTimeRange AggregateTimeRange;
        private DashboardSingleIntegerMetricComponent Chart;
        public TotalUniqueSongsMetric()
        {
            HeaderButtonConfigs = new List<HeaderButtonState>()
            {
                new HeaderButtonState(){Text = "7 Days",ButtonColor=Color.Secondary,Active=true, ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.SevenDays)) } ,
                new HeaderButtonState(){Text = "3 Months", ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.ThreeMonths)) } ,
                new HeaderButtonState(){Text = "One Year", ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.OneYear)) }
            };
            AggregateTimeRange = AggregateTimeRange.SevenDays;
        }
        private async Task UpdateChartDataTimeRange(AggregateTimeRange aggregateTimeRange)
        {
            AggregateTimeRange = aggregateTimeRange;
            await Chart.RefreshChartData();


        }

        private async Task<int> TotalUniqueSongs()
        {
            return await RadiocomDataAggregateDataClient.GetTotalUniqueSongsAsync(AggregateTimeRange);
        }

    }
}
