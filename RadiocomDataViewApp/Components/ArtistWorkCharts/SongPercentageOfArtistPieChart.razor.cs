using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Charts;
using Microsoft.AspNetCore.Components;
using RadiocomDataViewApp.Clients;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects;

namespace RadiocomDataViewApp.Components.ArtistWorkCharts
{
    public partial class SongPercentageOfArtistPieChart : ComponentBase
    {
        private DashboardPieChartComponent Chart;
        private List<HeaderButtonState> HeaderButtonConfigs { get; set; }

        private AggregateTimeRange ChartDataTimeRange = AggregateTimeRange.SevenDays;

        [Parameter]
        public int ArtistWorkId { get; set; }
        [Parameter]
        public string ArtistName { get; set; }

        [Parameter]
        public int ArtistId { get; set; }

        [Inject]
        public IRadiocomDataAggregateDataClient RadiocomDataAggregateDataClient { get; set; }
        [Inject]
        public NavigationManager NavManager { get; set; }

        public SongPercentageOfArtistPieChart()
        {
            HeaderButtonConfigs = new List<HeaderButtonState>()
            {
                 new HeaderButtonState(){Text = "7 Days",ButtonColor=Color.Secondary,Active=true, ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.SevenDays)) } ,
                new HeaderButtonState(){Text = "3 Months", ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.ThreeMonths)) } ,
                new HeaderButtonState(){Text = "All Time", ButtonClickCallback = EventCallback.Factory.Create(this, () => UpdateChartDataTimeRange(AggregateTimeRange.AllTime)) }
            };

            ChartDataTimeRange = AggregateTimeRange.SevenDays;
        }
        private void UpdateChartDataTimeRange(AggregateTimeRange mostPlayedTimeRange)
        {
            ChartDataTimeRange = mostPlayedTimeRange;
            Console.Write("djfoisajfiodsajfa\n\n\n");
            Chart.RefreshChartData();
        }
        private IEnumerable<DashboardChartData> SongPercentageOfArtist()
        {
            List<ItemCount> radioComData = RadiocomDataAggregateDataClient.GetSongPlayedAndOtherPlayed(ChartDataTimeRange, ArtistWorkId);
            return radioComData.Select(x => new DashboardChartData() { Label = x.Name, Value = x.Count, DataId = x.ItemId });

        }

        private ChartColor SliceColorGenerator(int indx)
        {
            ChartColor color;
            if (indx == 0)
            {
                color = OtherSongsPieChartColor;
            }
            else
            {
                color = ViewedSongPieChartColor;
            }
            return color;
        }
        private void OnDashboardPieChartClick()
        {
            NavManager.NavigateTo($"/artist/{ArtistId}/artistworks");
        }
        private static readonly ChartColor ViewedSongPieChartColor = ChartColor.FromRgba(255, 255, 255, 1);//white
        private static readonly ChartColor OtherSongsPieChartColor = ChartColor.FromRgba(104, 104, 103, 1);//grey
        
        private string DashboardPieChartComponentTitle => $" Comparison with Other Songs Played by {ArtistName}";

    }
}
