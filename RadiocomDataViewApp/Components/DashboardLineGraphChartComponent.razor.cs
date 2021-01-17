using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Charts;
using Microsoft.AspNetCore.Components;
using RadiocomDataViewApp.Objects;

namespace RadiocomDataViewApp.Components
{
    public partial class DashboardLineGraphChartComponent : ComponentBase
    {
        private const string DEFAULT_POINT_COLOR = "#FFF";
        private const string DEFAULT_FILL_COLOR = "#555FAF";

        private Object StandardTicks = new
        {
            FontColor = "#fff",
            BeginAtZero = true
        };
        public readonly ChartOptions chartOptions;
        public readonly object ChartOptionsObj;

        private LineChart<DashboardChartDatasetYValue> Chart;

        [Parameter]
        public string FillColor { get; set; }

        [Parameter]
        public string YAxisLabel { get; set; }
        [Parameter]
        public string XAxisLabel { get; set; }
        [Parameter]
        public string ChartTitle { get; set; }
        [Parameter]
        public Func<int, ChartColor> PointColorGenerator { get; set; }
        [Parameter]
        public Func<Task<IEnumerable<DashboardChartData>>> GenerateChartDatas { get; set; }
        [Parameter]
        public string ScaleLabelFontColor { get; set; }
        public DashboardLineGraphChartComponent()
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
                },
            };
            #endregion

        }

        public async Task RefreshChartData()
        {
            IEnumerable<DashboardChartData> newDatas = await GenerateChartDatas.Invoke();
            await Chart.Clear();
            await Chart.AddLabels(newDatas.Select(x => x.Label).ToArray());

            List<string> colors = new List<string>();
            for (int i = 0; i < newDatas.Count(); i++)
            {
                string lineColor = PointColorGenerator?.Invoke(i);
                if (!string.IsNullOrWhiteSpace(lineColor))
                {
                    colors.Add(lineColor);
                }
                else
                {
                    colors.Add(DEFAULT_POINT_COLOR);
                }
            }

            LineChartDataset<DashboardChartDatasetYValue> newChartDataset = new LineChartDataset<DashboardChartDatasetYValue>()
            {
                Data = newDatas.Select(x => new DashboardChartDatasetYValue() { Y = x.Value, DataId = x.DataId }).ToList(),
                Fill =  true,
                BackgroundColor = DEFAULT_FILL_COLOR,
                PointBackgroundColor = colors,
                PointBorderColor = colors,
                PointRadius = 3.5f
            };
            //newChartDataset.HoverBorderColor.Clear();
            CurrentDataset = newChartDataset;
            await Chart.AddDataSet(newChartDataset);
            await Chart.Update();
        }
        protected LineChartDataset<DashboardChartDatasetYValue> CurrentDataset { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await RefreshChartData();
            }
        }

        private string GetScaleFontColor()
        => string.IsNullOrWhiteSpace(ScaleLabelFontColor) ? "#fff" : ScaleLabelFontColor;
    }
}
