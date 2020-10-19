using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blazorise.Charts;
using Blazorise.Sidebar;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.CSharp;
namespace RadiocomDataViewApp.Components
{
    public partial class DashboardChartComponent : ComponentBase
    {
        private const string DEFAULT_BAR_COLOR = "#FFF";

        private Object StandardTicks = new
        {
            FontColor = "#fff",            
            BeginAtZero = true
        };

        
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public NavigationManager NavManager { get; set; }

        public readonly Object ChartOptionsObj;
        public readonly ChartOptions chartOptions;

        [Parameter]
        public string YAxisLabel { get; set; }
        [Parameter]
        public string XAxisLabel { get; set; }
        [Parameter]
        public string ChartTitle { get; set; }
        [Parameter]
        public Func<IEnumerable<DashboardChartData>> GenerateChartDatas { get; set; }
        [Parameter]
        public Func<int, ChartColor> BarColorGenerator { get; set; }
        [Parameter]
        public string ScaleLabelFontColor { get; set; }
        [Parameter]
        public string AxisTicksFontColor { get; set; }

        [Parameter]
        public EventCallback<ChartMouseEventArgs> OnBarElementClick { get; set; }

        public void RefreshChartData()
        {
            IEnumerable<DashboardChartData> newDatas = GenerateChartDatas?.Invoke();
            Chart.Clear();
            Chart.AddLabel(newDatas.Select(x => x.Label).ToArray());
            
            List<string> colors = new List<string>();
            for(int i = 0; i < newDatas.Count(); i++)
            {
                string barColor = BarColorGenerator?.Invoke(i);
                if (!string.IsNullOrWhiteSpace(barColor))
                {
                    colors.Add(barColor);
                }
                else
                {
                    colors.Add(DEFAULT_BAR_COLOR);
                }
            }

            BarChartDataset<int> newBarChartDataset = new BarChartDataset<int>()
            {
                Data = newDatas.Select(x => x.Value).ToList(),
                BackgroundColor = colors,
                BorderColor = colors
            };
            newBarChartDataset.HoverBackgroundColor.Clear();
            newBarChartDataset.HoverBorderColor.Clear();
            Chart.AddDataSet(newBarChartDataset);
            Chart.Update();
        }

        //public ChartOptions f()
        //{
        //    ChartOptions result;
        //    Axis f = new Axis();
            
        //}

        public DashboardChartComponent()
        {
            
            ChartOptionsObj = new
            {
                Legend = new { Display = false },
                Scales = new
                {
                    YAxes = new object[]
                    {
                        new
                        {
                            ScaleLabel = new
                            {
                                FontColor = GetScaleFontColor(),
                                Display = true,
                                LabelString = YAxisLabel ?? string.Empty
                            },
                            Ticks = StandardTicks

                        }
                    },
                    XAxes = new object[]
                    {
                        new
                        {
                            ScaleLabel = new
                            {
                                FontColor = GetScaleFontColor(),
                                Display = true,
                                LabelString = XAxisLabel ?? string.Empty
                            },
                            Ticks = StandardTicks
                        }
                    }
                }
            };


            chartOptions = new ChartOptions()
            {
                Scales = new Scales()
                {
                    YAxes = new List<Axis>() { new Axis() { Ticks = new AxeTicks() { FontColor = "#fff" }, ScaleLabel = new AxeScaleLabel() { FontColor = "#fff", Display = true, LabelString = "valueObj" } } },
                    XAxes = new List<Axis>() { new Axis() { Ticks = new AxeTicks() { FontColor = "#fff" }, ScaleLabel = new AxeScaleLabel() { FontColor = "#fff", Display = true, LabelString = "dimension" } } },
                }
            };
        }

        protected override bool ShouldRender()
        {

            return Chart.Data != null && (Chart.Data.Datasets?.Any(x => x.Data?.Any() ?? false) ?? false) && base.ShouldRender();
        }
        private string GetScaleFontColor()
        => string.IsNullOrWhiteSpace(ScaleLabelFontColor) ?  "#fff" : ScaleLabelFontColor;
    

    

        //private void BarClick(ChartMouseEventArgs args)
        //{

        //    //ClickMenu();
        //    Console.WriteLine("logging");
        //    //await Task.CompletedTask; //OnMenuHamburgerClick.InvokeAsync(args); 
        //}

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                RefreshChartData();
            }
        }
    }
    public class DashboardChartData
    {
        public string Label { get; set; }
        public int Value { get; set; }
        public int DataId { get; set; }
    }
}