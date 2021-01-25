using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Charts;
using Microsoft.AspNetCore.Components;
using RadiocomDataViewApp.Clients;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects;

namespace RadiocomDataViewApp.Components.ArtistCharts
{
    public partial class ArtistPercentageOfArtistSongsPieChart : ComponentBase
    {
        private DashboardPieChartComponent Chart;
        private List<HeaderButtonState> HeaderButtonConfigs { get; set; }

        private AggregateTimeRange ChartDataTimeRange = AggregateTimeRange.SevenDays;

        [Parameter]
        public string ArtistName { get; set; }

        [Parameter]
        public int ArtistId { get; set; }

        [Inject]
        public IRadiocomDataAggregateDataClient RadiocomDataAggregateDataClient { get; set; }
        [Inject]
        public NavigationManager NavManager { get; set; }

        public ArtistPercentageOfArtistSongsPieChart()
        {
            HeaderButtonConfigs = new List<HeaderButtonState>()
            {
                 new HeaderButtonState(){Text = "7 Days",ButtonColor=Color.Secondary,Active=true, ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.SevenDays)) } ,
                new HeaderButtonState(){Text = "3 Months", ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.ThreeMonths)) } ,
                new HeaderButtonState(){Text = "All Time", ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.AllTime)) }
            };

            ChartDataTimeRange = AggregateTimeRange.SevenDays;
        }

        private async Task UpdateChartDataTimeRange(AggregateTimeRange mostPlayedTimeRange)
        {
            ChartDataTimeRange = mostPlayedTimeRange;
           await Chart.RefreshChartData();
        }
        private async Task<IEnumerable<DashboardChartData>> ArtistPercentageOfArtistSongs()
        {
            List<ItemCount> radioComData = await RadiocomDataAggregateDataClient.GetArtistSongsPlayed(ChartDataTimeRange, ArtistId);
            CurrentDataset = radioComData;
            return radioComData.Select(x => new DashboardChartData() { Label = x.Name, Value = x.Count, DataId = x.ItemId });

        }

        private List<ItemCount> CurrentDataset;
        private const decimal SLICE_SHADE_STEP = 0.2023809523809524m;//https://stackoverflow.com/a/40619637
        
        private ChartColor SliceColorGenerator (int index) =>
            ChartColor.FromRgba(
                 (byte)((index + 1) * 26 * SLICE_SHADE_STEP),
                (byte)((index + 1) * 35 * SLICE_SHADE_STEP) , 
                (byte)((index + 1) * 126 * SLICE_SHADE_STEP), 
                1);
        

        private void OnDashboardPieChartClick(DashboardChartMouseEventArgs eventArgs)
        {
            int songId = CurrentDataset[eventArgs.Index].ItemId;            
            NavManager.NavigateTo($"artistwork/{songId}");
        }
        private string DashboardPieChartComponentTitle => $"Comparison of Songs by {ArtistName}";

    }
}
