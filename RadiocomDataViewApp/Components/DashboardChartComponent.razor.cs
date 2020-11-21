﻿using System;
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
        public Dictionary<string, EventCallback<MouseEventArgs>> ChartTimeFrames { get; set; }
        [Parameter]
        public EventCallback<DashboardChartMouseEventArgs> OnDashboardChartElementClick { get; set; }

        private EventCallback<ChartMouseEventArgs> OnBarElementClick { get; set;}
        private async Task OnBarElementClickedHandler(ChartMouseEventArgs args)
        {
            
            DashboardChartMouseEventArgs chartMouseEventArgs = new DashboardChartMouseEventArgs(args.DatasetIndex, args.Index, args.Model);
            chartMouseEventArgs.DatasetElement = CurrentDataset.Data[args.Index]; 
            await OnDashboardChartElementClick.InvokeAsync(chartMouseEventArgs);
            Console.WriteLine("here");
            
        }

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

            BarChartDataset<Test> newBarChartDataset = new BarChartDataset<Test>()
            {
                Data = newDatas.Select(x => new Test() { X = x.Value, DataId = x.DataId }).ToList(),
                BackgroundColor = colors,
                BorderColor = colors
            };
            newBarChartDataset.HoverBackgroundColor.Clear();
            newBarChartDataset.HoverBorderColor.Clear();
            CurrentDataset = newBarChartDataset;
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
                }
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
            Console.WriteLine(Chart.Data != null && (Chart.Data.Datasets?.Any(x => x.Data?.Any() ?? false) ?? false) && base.ShouldRender());
            return Chart.Data != null && (Chart.Data.Datasets?.Any(x => x.Data?.Any() ?? false) ?? false) && base.ShouldRender();
        }
        private string GetScaleFontColor()
        => string.IsNullOrWhiteSpace(ScaleLabelFontColor) ?  "#fff" : ScaleLabelFontColor;
        protected BarChartDataset<Test> CurrentDataset { get; set; }


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