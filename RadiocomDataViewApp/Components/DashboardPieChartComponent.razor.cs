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
        public  object ChartOptionsObj;



        [Parameter]
        public string ChartTitle { get; set; }

        [Parameter]
        public Func<Task<IEnumerable<DashboardChartData>>> GenerateChartDatas { get; set; }
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
                Legend = new { Display = true },

                AspectRatio = .75

            };
            #endregion chartOptions
        }

        public async Task RefreshChartData()
        {
            Console.WriteLine("refresh picharet");
            IEnumerable<DashboardChartData> newDatas = await GenerateChartDatas.Invoke();
            await Chart.Clear();
            await Chart.AddLabels(newDatas.Select(x => x.Label).ToArray());

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
            await Chart.AddDataSet(newChartDataset);
            await Chart.Update();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await RefreshChartData();
            }
        }
       

        private async Task OnPieChartClickHandler(ChartMouseEventArgs args)
        {
            DashboardChartMouseEventArgs chartMouseEventArgs = new DashboardChartMouseEventArgs(args.DatasetIndex, args.Index, args.Model);
            chartMouseEventArgs.DatasetElement = CurrentDataset.Data[chartMouseEventArgs.Index];
            await OnDashboardPieChartClick.InvokeAsync(chartMouseEventArgs);
        }

        protected PieChartDataset<int> CurrentDataset { get; set; }
    }
}
