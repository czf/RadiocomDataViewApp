
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Charts;
using Blazorise.Sidebar;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using RadiocomDataViewApp.Clients;
using RadiocomDataViewApp.Components;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects;
using static RadiocomDataViewApp.Components.ChartComponentHeaderButtons;
using static RadiocomDataViewApp.Components.DashboardChartComponent;

namespace RadiocomDataViewApp.Pages
{
    public partial class Index : ComponentBase
    {




        [CascadingParameter]
        public EventCallback OnMenuHamburgerClick { get; set; }
        [CascadingParameter]
        public Action ClickMenu { get; set; }

        public List<HeaderButtonState> HeaderButtonConfigs { get; set; }
        public Index()
        {
            HeaderButtonConfigs = new List<HeaderButtonState>()
            {
                new HeaderButtonState(){Text = "7 Days",ButtonColor=Color.Secondary,Active=true, ButtonClickCallback = EventCallback.Factory.Create(this, () => ChangeChartTimeRange(MostPlayedSongsTimeRange.SevenDays))},
                new HeaderButtonState(){Text = "3 Months", ButtonClickCallback = EventCallback.Factory.Create(this, async() => {await Task.Delay(5000); ChangeChartTimeRange(MostPlayedSongsTimeRange.ThreeMonths); }) } ,
                new HeaderButtonState(){Text = "All Time", ButtonClickCallback = EventCallback.Factory.Create(this, () => ChangeChartTimeRange(MostPlayedSongsTimeRange.AllTime))}
            };


        }

        

        //private void BarClick(ChartMouseEventArgs args)
        private void BarClick(MouseEventArgs args)
        {

            ClickMenu();
        }
        private void BarClick2()
        {

            ClickMenu();
        }


        protected override Task OnParametersSetAsync()
        {
            return base.OnParametersSetAsync();
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();

        }

        private IEnumerable<DashboardChartData> TopPlayedSongs()
        {
            MostPlayedSongsTimeRange range = GetMostPlayedSongsTimeRange();
            List<ItemCount> radioComData = RadiocomDataAggregateDataClient.GetMostPlayedSongs(range);
            return radioComData.Select(x => new DashboardChartData() { Label = x.Name, Value = x.Count, DataId = x.ItemId });
        }

        private void ClickBar(DashboardChartMouseEventArgs args)
        {
            BarChartDatasetXValue element = (BarChartDatasetXValue)args.DatasetElement;
            NavManager.NavigateTo($"artistwork/{element.DataId}");
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //await HandleRedraw();
            }
        }



        BarChartDataset<double> GetChartDataset()
        {
            List<string> f2 = new List<string>() { initialChartColor, alternateChartColor, initialChartColor, alternateChartColor, initialChartColor, alternateChartColor };//{ ChartColor.FromRgba(52, 53,52, 1f), ChartColor.FromRgba(67, 65, 65, 1f), ChartColor.FromRgba(52, 53, 52, 0.2f), ChartColor.FromRgba(52, 53, 52, 0.2f), ChartColor.FromRgba(52, 53, 52, 0.2f), ChartColor.FromRgba(52, 53, 52, 0.2f) };

            List<string> f = new List<string>() { alternateChartColor, initialChartColor, alternateChartColor, initialChartColor, alternateChartColor, initialChartColor };//{ ChartColor.FromRgba(255, 99, 132, 0.2f), ChartColor.FromRgba(255, 99, 132, 0.2f), ChartColor.FromRgba(255, 99, 132, 0.2f), ChartColor.FromRgba(255, 99, 132, 0.2f), ChartColor.FromRgba(255, 99, 132, 0.2f), ChartColor.FromRgba(255, 99, 132, 0.2f) };


            var set = new BarChartDataset<double>
            {
                //Label = "# of randoms",
                Data = RandomizeData().OrderByDescending(x => x).ToList(),
                BackgroundColor = f2,
                BorderColor = f2
                //Fill = true,
                //PointRadius = 2,
                //BorderDash = new List<int> { }
            };

            set.HoverBackgroundColor.Clear();
            set.HoverBorderColor.Clear();
            return set;
        }

        ChartColor initialChartColor = ChartColor.FromRgba(104, 103, 103, 1f);
        ChartColor alternateChartColor = ChartColor.FromRgba(170, 165, 165, 1f);

        string[] Labels = { "Red", "Blue", "Yellow", "Green", "Purple", "Orange" };
        //List<string> backgroundColors = new List<string> { ChartColor.FromRgba(255, 99, 132, 0.2f), ChartColor.FromRgba(54, 162, 235, 0.2f),  ChartColor.FromRgba(255, 206, 86, 0.2f), ChartColor.FromRgba(75, 192, 192, 0.2f), ChartColor.FromRgba(153, 102, 255, 0.2f), ChartColor.FromRgba(255, 159, 64, 0.2f) };
        //List<string> borderColors = new List<string> {  ChartColor.FromRgba(255, 99, 132, 1f), ChartColor.FromRgba(54, 162, 235, 1f), ChartColor.FromRgba(255, 206, 86, 1f), ChartColor.FromRgba(75, 192, 192, 1f), ChartColor.FromRgba(153, 102, 255, 1f), ChartColor.FromRgba(255, 159, 64, 1f) };

        List<double> RandomizeData()
        {
            var r = new Random(DateTime.Now.Millisecond);

            return new List<double> { r.Next(3, 50) * r.NextDouble(), r.Next(3, 50) * r.NextDouble(), r.Next(3, 50) * r.NextDouble(), r.Next(3, 50) * r.NextDouble(), r.Next(3, 50) * r.NextDouble(), r.Next(3, 50) * r.NextDouble() };
        }


        private MostPlayedSongsTimeRange GetMostPlayedSongsTimeRange()
        {
            Console.WriteLine("");
            return MostPlayedSongsTimeRangeValue;
        }

        private RenderFragment GenerateButton()
        {
            RenderFragment renderFragmentResult = new RenderFragment(GenerateButtonDelegateFunction);
            return renderFragmentResult; 
        }
        
        private void GenerateButtonDelegateFunction(RenderTreeBuilder builder)
        {
            builder.OpenComponent<Button>(0);
            builder.AddAttribute(1, "Active", true);
            builder.AddAttribute(2, "Outline", true);
            builder.AddAttribute(3, "Color", Color.Secondary);
            builder.AddAttribute(7, "ChildContent", ChildContentTextBuilder(8, "7 Days"));
            
            builder.CloseComponent();
        }

        private RenderFragment ChildContentTextBuilder(int sequence, string text)
        {
            return (b) => b.AddContent(sequence, text);
        }
        private Dictionary<string, EventCallback> HeaderButtons = new Dictionary<string, EventCallback>();

        private MostPlayedSongsTimeRange MostPlayedSongsTimeRangeValue = MostPlayedSongsTimeRange.SevenDays;
        private void ChangeChartTimeRange(MostPlayedSongsTimeRange mostPlayedSongsTimeRange)
        {
            Console.WriteLine("change range:" + mostPlayedSongsTimeRange);
            MostPlayedSongsTimeRangeValue = mostPlayedSongsTimeRange;
            TopPlayedSongsChart.RefreshChartData();
        }
        DashboardChartComponent TopPlayedSongsChart;

    }

    public static class ChartColorExtensions
    {
        public static ChartColor ShallowClone(this ChartColor chartColor)
            => ChartColor.FromRgba(chartColor.R, chartColor.B, chartColor.G, chartColor.A);
        
    }

}

