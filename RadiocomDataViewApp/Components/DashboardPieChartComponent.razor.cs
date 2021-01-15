using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Charts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RadiocomDataViewApp.Objects;

namespace RadiocomDataViewApp.Components
{
    public partial class DashboardPieChartComponent : ComponentBase
    {
        private const string DEFAULT_SLICE_COLOR = "#FFF";

        private PieChart<int> Chart;
        public readonly object ChartOptionsObj;



        [Parameter]
        public string ChartTitle { get; set; }
        [Parameter]
        public Func<IEnumerable<DashboardChartData>> GenerateChartDatas { get; set; }
        [Parameter]
        public Func<int, ChartColor> SliceColorGenerator { get; set; }
        
        /// <summary>
        /// event for clicking specfically on the PieChart
        /// </summary>
        [Parameter]
        public EventCallback<DashboardChartMouseEventArgs> OnDashboardPieChartClick { get; set; }

        public DashboardPieChartComponent()
        {
            #region chartOptions
            ChartOptionsObj = new
            {
                Legend = new { Display = false },
                //Scales = new
                //{
                //    YAxes = new object[]
                //    {
                //        new
                //        {
                //            ScaleLabel = new
                //            {
                //                FontColor = GetScaleFontColor(),
                //                Display = true,
                //                LabelString = YAxisLabel ?? string.Empty
                //            },
                //            Ticks = StandardTicks

                //        }
                //    },
                //    XAxes = new object[]
                //    {
                //        new
                //        {
                //            ScaleLabel = new
                //            {
                //                FontColor = GetScaleFontColor(),
                //                Display = true,
                //                LabelString = XAxisLabel ?? string.Empty
                //            },
                //            Ticks = StandardTicks
                //        }
                //    }
                //},
                AspectRatio = 1.5

            };
            #endregion chartOptions
        }

        public void RefreshChartData()
        {
            Console.WriteLine("refresh picharet");
            IEnumerable<DashboardChartData> newDatas = GenerateChartDatas?.Invoke();
            Chart.Clear();
            Chart.AddLabels(newDatas.Select(x => x.Label).ToArray());

            List<string> colors = new List<string>();
            for (int i = 0; i < newDatas.Count(); i++)
            {
                string sliceColor = SliceColorGenerator?.Invoke(i);
                if (!string.IsNullOrWhiteSpace(sliceColor))
                {
                    colors.Add(sliceColor);
                }
                else
                {
                    colors.Add(DEFAULT_SLICE_COLOR);
                }
            }

            PieChartDataset<int> newChartDataset = new PieChartDataset<int>()
            {
                Data = newDatas.Select(x => x.Value).ToList(),
                BackgroundColor = colors,
                BorderColor = colors
            };
            newChartDataset.HoverBackgroundColor.Clear();
            newChartDataset.HoverBorderColor.Clear();
            CurrentDataset = newChartDataset;
            Chart.AddDataSet(newChartDataset);
            Chart.Update();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                Console.Write("ridnejejf");
                RefreshChartData();
            }
        }

        //private async Task OnSliceElementClickedHandler(ChartMouseEventArgs args)
        //{

        //    DashboardChartMouseEventArgs chartMouseEventArgs = new DashboardChartMouseEventArgs(args.DatasetIndex, args.Index, args.Model);
        //    chartMouseEventArgs.DatasetElement = CurrentDataset.Data[args.Index];
        //    await OnDashboardChartElementClick.InvokeAsync(chartMouseEventArgs);
        //}

        private async Task OnPieChartClick(ChartMouseEventArgs args)
        {
            DashboardChartMouseEventArgs chartMouseEventArgs = new DashboardChartMouseEventArgs(args.DatasetIndex, args.Index, args.Model);
            chartMouseEventArgs.DatasetElement = CurrentDataset.Data[chartMouseEventArgs.Index];
            await OnDashboardPieChartClick.InvokeAsync(chartMouseEventArgs);
        }

        protected PieChartDataset<int> CurrentDataset { get; set; }
    }
}
