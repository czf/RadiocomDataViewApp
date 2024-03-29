﻿using System;
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

        private PieChart<long> Chart;
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
            IEnumerable<DashboardChartData> newDatas = await GenerateChartDatas.Invoke();
            await Chart.Clear();
            await Chart.AddLabels(newDatas.Select(x => x.Label).ToArray());

            List<string> colors = new List<string>();
            for (int i = 0; i < newDatas.Count(); i++)
            {
                string sliceColor = null;
                if (i < 9) 
                {
                    sliceColor = SliceColorGenerator?.Invoke(i); 
                }

                if (!string.IsNullOrWhiteSpace(sliceColor))
                {
                    colors.Add(sliceColor);
                }
                else
                {
                    colors.Add(DEFAULT_SLICE_COLOR);
                }
            }

            PieChartDataset<long> newChartDataset = new PieChartDataset<long>()
            {
                Data = newDatas.Select(x => x.Value).ToList(),
                BackgroundColor = colors,
                BorderColor = colors
            };
            CurrentDataset = newChartDataset;
            await Chart.AddDatasetsAndUpdate(newChartDataset);
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender).ContinueWith(x =>
            InvokeAsync(async () => {
                if (firstRender)
                {
                    await RefreshChartData();
                }

                if(Chart.Data.Labels.Count > 7)
                {
                    ChartOptionsObj = new
                    {
                        Legend = new { Display = true },

                        AspectRatio = .55

                    };
                    await Chart.SetOptionsObject(ChartOptionsObj);
                    await Chart.Update();
                    await Chart.Resize();
                }

            })).Unwrap();
        }
       

        private async Task OnPieChartClickHandler(ChartMouseEventArgs args)
        {
            DashboardChartMouseEventArgs chartMouseEventArgs = new DashboardChartMouseEventArgs(args.DatasetIndex, args.Index, args.Model);
            chartMouseEventArgs.DatasetElement = CurrentDataset.Data[chartMouseEventArgs.Index];
            await OnDashboardPieChartClick.InvokeAsync(chartMouseEventArgs);
        }

        protected PieChartDataset<long> CurrentDataset { get; set; }
    }
}
