using Paramecium.Engine;
using Paramecium.Rendering;

namespace Paramecium.Forms
{
    public partial class FormStatistics : Form
    {
        private Bitmap _graphViewImage;

        public FormStatistics()
        {
            InitializeComponent();

            ComboBoxDataType.SelectedIndex = 0;
            ComboBoxTimeSpan.SelectedIndex = 0;

            _graphViewImage = new Bitmap(1, 1);
        }

        private async void FormStatistics_Shown(object sender, EventArgs e)
        {
            while (true)
            {
                StatisticsGraphRenderer.RenderedGraphInformation graphInformation = new StatisticsGraphRenderer.RenderedGraphInformation();

                Bitmap prevFrameGraphViewImage = _graphViewImage;

                _graphViewImage = new Bitmap(GraphView.Width, GraphView.Height);

                int histogramBinCount = int.Max(1, (GraphView.Width - 1) / 10);

                Soup? soup = Globals.Soup;

                if (ComboBoxDataType.Text.Contains("Distribution"))
                {
                    LabelTimeSpan.Enabled = false;
                    ComboBoxTimeSpan.Enabled = false;
                }
                if (ComboBoxDataType.Text.Contains("Change Over Time"))
                {
                    LabelTimeSpan.Enabled = true;
                    ComboBoxTimeSpan.Enabled = true;
                }

                if (soup is not null)
                {
                    if (ComboBoxDataType.Text.Contains("Distribution"))
                    {
                        switch (ComboBoxDataType.Text)
                        {
                            case "Total Mutation Count Distribution":
                                graphInformation = StatisticsGraphRenderer.DrawHistogram(_graphViewImage, soup.SoupStatistics.DistributionData.MutationCount, histogramBinCount);
                                break;
                            case "Generation Distribution":
                                graphInformation = StatisticsGraphRenderer.DrawHistogram(_graphViewImage, soup.SoupStatistics.DistributionData.Generation, histogramBinCount);
                                break;
                            case "Age Distribution":
                                graphInformation = StatisticsGraphRenderer.DrawHistogram(_graphViewImage, soup.SoupStatistics.DistributionData.Age, histogramBinCount);
                                break;
                            case "Offspring Count Distribution":
                                graphInformation = StatisticsGraphRenderer.DrawHistogram(_graphViewImage, soup.SoupStatistics.DistributionData.OffspringCount, histogramBinCount);
                                break;
                            case "Velocity Magnitude Distribution":
                                graphInformation = StatisticsGraphRenderer.DrawHistogram(_graphViewImage, soup.SoupStatistics.DistributionData.VelocityMagnitude, histogramBinCount);
                                break;
                            case "Angular Velocity Distribution":
                                graphInformation = StatisticsGraphRenderer.DrawHistogram(_graphViewImage, soup.SoupStatistics.DistributionData.AngularVelocity, histogramBinCount);
                                break;
                            case "Element Amount Distribution":
                                graphInformation = StatisticsGraphRenderer.DrawHistogram(_graphViewImage, soup.SoupStatistics.DistributionData.Element, histogramBinCount);
                                break;
                            case "Element In/Outflow Distribution":
                                graphInformation = StatisticsGraphRenderer.DrawHistogram(_graphViewImage, soup.SoupStatistics.DistributionData.ElementInOutflow, histogramBinCount);
                                break;
                            case "Reproduction Progress Distribution":
                                graphInformation = StatisticsGraphRenderer.DrawHistogram(_graphViewImage, soup.SoupStatistics.DistributionData.ReproductionProgress, histogramBinCount);
                                break;
                            case "Reproduction Rate Distribution":
                                graphInformation = StatisticsGraphRenderer.DrawHistogram(_graphViewImage, soup.SoupStatistics.DistributionData.ReproductionRate, histogramBinCount);
                                break;
                            case "Brain Eat Output Distribution":
                                graphInformation = StatisticsGraphRenderer.DrawHistogram(_graphViewImage, soup.SoupStatistics.DistributionData.BrainEatOutput, histogramBinCount);
                                break;
                            case "Brain Attack Output Distribution":
                                graphInformation = StatisticsGraphRenderer.DrawHistogram(_graphViewImage, soup.SoupStatistics.DistributionData.BrainAttackOutput, histogramBinCount);
                                break;
                        }
                    }
                    if (ComboBoxDataType.Text.Contains("Change Over Time"))
                    {
                        long startTime = soup.SoupStatistics.ChangeOverTimeData.Datas[ComboBoxTimeSpan.SelectedIndex].GetStartTimestep();
                        long endTime = soup.SoupStatistics.ChangeOverTimeData.Datas[ComboBoxTimeSpan.SelectedIndex].GetEndTimestep();
                        List<SoupStatistics.ChangeOverTimeDatas.DataEntry> dataEntries = soup.SoupStatistics.ChangeOverTimeData.Datas[ComboBoxTimeSpan.SelectedIndex].GetDataEntries();

                        switch (ComboBoxDataType.Text)
                        {
                            case "Animal Population Change Over Time":
                                graphInformation = StatisticsGraphRenderer.DrawLineChart(_graphViewImage, SoupStatistics.ChangeOverTimeDatas.DataEntry.GetAnimalPopulationData(dataEntries), startTime, endTime);
                                break;
                            case "Latest Generation Change Over Time":
                                graphInformation = StatisticsGraphRenderer.DrawLineChart(_graphViewImage, SoupStatistics.ChangeOverTimeDatas.DataEntry.GetLatestGenerationData(dataEntries), startTime, endTime);
                                break;
                            case "Max Total Mutation Count Change Over Time":
                                graphInformation = StatisticsGraphRenderer.DrawLineChart(_graphViewImage, SoupStatistics.ChangeOverTimeDatas.DataEntry.GetMaxMutationCountData(dataEntries), startTime, endTime);
                                break;
                        }
                    }
                }

                if (graphInformation.XValueType == StatisticsGraphRenderer.GraphValueType.Long)
                {
                    LabelXMinValue.Text = $"{graphInformation.XMin.ToString("0")}";
                    LabelXMaxValue.Text = $"{graphInformation.XMax.ToString("0")}";
                }
                if (graphInformation.XValueType == StatisticsGraphRenderer.GraphValueType.Double)
                {
                    LabelXMinValue.Text = $"{graphInformation.XMin.ToString("0.000000")}";
                    LabelXMaxValue.Text = $"{graphInformation.XMax.ToString("0.000000")}";
                }

                if (graphInformation.YValueType == StatisticsGraphRenderer.GraphValueType.Long)
                {
                    LabelYMinValue.Text = $"{graphInformation.YMin.ToString("0")}";
                    LabelYMaxValue.Text = $"{graphInformation.YMax.ToString("0")}";
                }
                if (graphInformation.YValueType == StatisticsGraphRenderer.GraphValueType.Double)
                {
                    LabelYMinValue.Text = $"{graphInformation.YMin.ToString("0.000000")}";
                    LabelYMaxValue.Text = $"{graphInformation.YMax.ToString("0.000000")}";
                }

                GraphView.Image = _graphViewImage;

                prevFrameGraphViewImage.Dispose();

                await Task.Delay(100);
            }
        }
    }
}
