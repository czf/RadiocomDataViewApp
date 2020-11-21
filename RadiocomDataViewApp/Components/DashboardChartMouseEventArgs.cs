using Blazorise.Charts;

namespace RadiocomDataViewApp.Components
{
    public class DashboardChartMouseEventArgs : ChartMouseEventArgs
    {
        public DashboardChartMouseEventArgs(int datasetIndex, int index, object model) : base(datasetIndex, index, model){}
        public object DatasetElement { get; set; }
    }
}