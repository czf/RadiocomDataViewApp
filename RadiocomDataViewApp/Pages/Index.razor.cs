
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Charts;
using Blazorise.Sidebar;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RadiocomDataViewApp.Components;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects;

namespace RadiocomDataViewApp.Pages
{
    public partial class Index : ComponentBase
    {


        
        
        [CascadingParameter]
        public EventCallback OnMenuHamburgerClick { get; set; }
        [CascadingParameter]
        public Action ClickMenu { get; set; }
      

        

        //private void BarClick(ChartMouseEventArgs args)
        private void BarClick(MouseEventArgs args)
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
            
            List<ItemCount> radioComData = RadiocomDataAggregateDataClient.GetMostPlayedSongs();
            return radioComData.Select(x => new DashboardChartData() { Label = x.Name, Value = x.Count });
        }

        private void ClickBar(ChartMouseEventArgs args)
        {
            ChartData
            Console.WriteLine("here");
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //await HandleRedraw();
            }
        }

        //private async Task HandleRedraw()
        //{
        //    await Chart.Clear();
        //    await Chart.AddLabel(Labels);
        //    await Chart.AddDataSet(GetChartDataset());
        //    await Chart.Update();
            
        //}

        BarChartDataset<double> GetChartDataset()
        {
            List<string> f2 = new List<string>() { initialChartColor, alternateChartColor, initialChartColor, alternateChartColor, initialChartColor, alternateChartColor };//{ ChartColor.FromRgba(52, 53,52, 1f), ChartColor.FromRgba(67, 65, 65, 1f), ChartColor.FromRgba(52, 53, 52, 0.2f), ChartColor.FromRgba(52, 53, 52, 0.2f), ChartColor.FromRgba(52, 53, 52, 0.2f), ChartColor.FromRgba(52, 53, 52, 0.2f) };

            List<string> f = new List<string>() {  alternateChartColor, initialChartColor, alternateChartColor, initialChartColor, alternateChartColor, initialChartColor};//{ ChartColor.FromRgba(255, 99, 132, 0.2f), ChartColor.FromRgba(255, 99, 132, 0.2f), ChartColor.FromRgba(255, 99, 132, 0.2f), ChartColor.FromRgba(255, 99, 132, 0.2f), ChartColor.FromRgba(255, 99, 132, 0.2f), ChartColor.FromRgba(255, 99, 132, 0.2f) };


            var set  = new BarChartDataset<double>
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
    }

    public static class ChartColorExtensions
    {
        public static ChartColor ShallowClone(this ChartColor chartColor)
            => ChartColor.FromRgba(chartColor.R, chartColor.B, chartColor.G, chartColor.A);
        
    }

}

