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
using Microsoft.AspNetCore.Components.Web;
using RadiocomDataViewApp.Pages;
using RadiocomDataViewApp.Objects;

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



        //[Inject]
        //public IJSRuntime JSRuntime { get; set; }
        //[Inject]
        //public NavigationManager NavManager { get; set; }

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
        public Dictionary<string, EventCallback<MouseEventArgs>> ChartTimeFrames { get; set; }
        [Parameter]
        public EventCallback<DashboardChartMouseEventArgs> OnDashboardChartElementClick { get; set; }

        private EventCallback<ChartMouseEventArgs> OnBarElementClick { get; set;}
        private async Task OnBarElementClickedHandler(ChartMouseEventArgs args)
        {
            
            DashboardChartMouseEventArgs chartMouseEventArgs = new DashboardChartMouseEventArgs(args.DatasetIndex, args.Index, args.Model);
            chartMouseEventArgs.DatasetElement = CurrentDataset.Data[args.Index]; 
            await OnDashboardChartElementClick.InvokeAsync(chartMouseEventArgs);
        }

        

        public void RefreshChartData()
        {
            IEnumerable<DashboardChartData> newDatas = GenerateChartDatas?.Invoke();
            Chart.Clear();
            Chart.AddLabels(newDatas.Select(x => x.Label).ToArray());
            
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

            BarChartDataset<BarChartDatasetXValue> newBarChartDataset = new BarChartDataset<BarChartDatasetXValue>()
            {
                Data = newDatas.Select(x => new BarChartDatasetXValue() { X = x.Value, DataId = x.DataId }).ToList(),
                BackgroundColor = colors,
                BorderColor = colors
            };
            newBarChartDataset.HoverBackgroundColor.Clear();
            newBarChartDataset.HoverBorderColor.Clear();
            CurrentDataset = newBarChartDataset;
            Chart.AddDataSet(newBarChartDataset);
            Chart.Update();
        }

        private HorizontalBarChart<BarChartDatasetXValue> Chart;
        public DashboardChartComponent()
        {
            #region chartOptions
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
                },
                AspectRatio = 1.5
                
            };


            chartOptions = new ChartOptions()
            {
                Scales = new Scales()
                {
                    YAxes = new List<Axis>() { new Axis() { Ticks = new AxisTicks() { FontColor = "#fff" }, ScaleLabel = new AxisScaleLabel() { FontColor = "#fff", Display = true, LabelString = "valueObj" } } },
                    XAxes = new List<Axis>() { new Axis() { Ticks = new AxisTicks() { FontColor = "#fff" }, ScaleLabel = new AxisScaleLabel() { FontColor = "#fff", Display = true, LabelString = "dimension" } } },
                }
            };
            #endregion
 
        }

        protected override bool ShouldRender()
        {
            Console.WriteLine("should render:" + Chart.Data != null && (Chart.Data.Datasets?.Any(x => x.Data?.Any() ?? false) ?? false) && base.ShouldRender());
            return Chart.Data != null && (Chart.Data.Datasets?.Any(x => x.Data?.Any() ?? false) ?? false) && base.ShouldRender();
        }
        private string GetScaleFontColor()
        => string.IsNullOrWhiteSpace(ScaleLabelFontColor) ?  "#fff" : ScaleLabelFontColor;
        protected BarChartDataset<BarChartDatasetXValue> CurrentDataset { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                RefreshChartData();
            }
        }
    }
    
}