using Paramecium.Variables;

namespace Paramecium.Rendering
{
    public static class StatisticsGraphRenderer
    {
        public static void DrawLineChart(Bitmap graphViewImage, List<double> values)
        {
            Int2d graphSize = new Int2d(graphViewImage.Width - 2, graphViewImage.Height - 2);

            if (graphSize.X >= 1 && graphSize.Y >= 1)
            {
                Graphics graphics = Graphics.FromImage(graphViewImage);

                graphics.DrawLine(Pens.Yellow, 0, graphViewImage.Height - 1, graphViewImage.Width - 1, graphViewImage.Height - 1);
                graphics.DrawLine(Pens.Yellow, graphViewImage.Width - 1, 0, graphViewImage.Width - 1, graphViewImage.Height - 1);

                if (values.Count >= 2)
                {
                    for (int i = 1; i < values.Count; i++)
                    {
                        int startX = (int)double.Floor(graphSize.X / (double)(values.Count - 1) * (i - 1));
                        int endX = (int)double.Floor(graphSize.X / (double)(values.Count - 1) * i);
                        int startY = (int)double.Floor(graphSize.Y * (1d - values[i - 1]));
                        int endY = (int)double.Floor(graphSize.Y * (1d - values[i]));

                        graphics.DrawLine(Pens.White, startX, startY, endX, endY);
                    }
                }

                graphics.Dispose();
            }
        }

        public static RenderedGraphInformation DrawLineChart(Bitmap graphViewImage, List<long> values, long startT, long endT)
        {
            if (values.Count > 0)
            {
                long minValue = long.MaxValue;
                long maxValue = long.MinValue;

                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i] < minValue) minValue = values[i];
                    if (values[i] > maxValue) maxValue = values[i];
                }

                List<double> dataValues = new List<double>();
                for (int i = 0; i < values.Count; i++)
                {
                    dataValues.Add(values[i] / (double)maxValue);
                }

                DrawLineChart(graphViewImage, dataValues);

                return new RenderedGraphInformation(startT, endT, GraphValueType.Long, 0, maxValue, GraphValueType.Long);
            }
            else return new RenderedGraphInformation(0, 0, GraphValueType.Long, 0, 0, GraphValueType.Long);
        }
        public static RenderedGraphInformation DrawLineChart(Bitmap graphViewImage, List<double> values, long startT, long endT)
        {
            if (values.Count > 0)
            {
                double minValue = double.MaxValue;
                double maxValue = double.MinValue;

                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i] < minValue) minValue = values[i];
                    if (values[i] > maxValue) maxValue = values[i];
                }

                List<double> dataValues = new List<double>();
                for (int i = 0; i < values.Count; i++)
                {
                    dataValues.Add(values[i] / (double)maxValue);
                }

                DrawLineChart(graphViewImage, dataValues);

                return new RenderedGraphInformation(startT, endT, GraphValueType.Long, 0, maxValue, GraphValueType.Long);
            }
            else return new RenderedGraphInformation(0, 0, GraphValueType.Long, 0, 0, GraphValueType.Long);
        }

        public static void DrawHistogram(Bitmap graphViewImage, List<double> binValues)
        {
            Int2d graphSize = new Int2d(graphViewImage.Width - 1, graphViewImage.Height - 1);

            if (graphSize.X >= 1 && graphSize.Y >= 1)
            {
                Graphics graphics = Graphics.FromImage(graphViewImage);

                graphics.DrawLine(Pens.Yellow, 0, graphViewImage.Height - 1, graphViewImage.Width - 1, graphViewImage.Height - 1);
                graphics.DrawLine(Pens.Yellow, graphViewImage.Width - 1, 0, graphViewImage.Width - 1, graphViewImage.Height - 1);

                for (int i = 0; i < binValues.Count; i++)
                {
                    int startX = (int)double.Ceiling(graphSize.X / (double)binValues.Count * i) + 1;
                    int endX = (int)double.Floor(graphSize.X / (double)binValues.Count * (i + 1));

                    if (endX - startX >= 1)
                    {
                        graphics.FillRectangle(Brushes.White, startX, graphSize.Y - (int)double.Ceiling(graphSize.Y * binValues[i]), endX - startX, (int)double.Ceiling(graphSize.Y * binValues[i]));
                    }
                }

                graphics.Dispose();
            }
        }

        public static RenderedGraphInformation DrawHistogram(Bitmap graphViewImage, List<long> values, int maxBinCount)
        {
            if (values.Count > 0)
            {
                long minValue = long.MaxValue;
                long maxValue = long.MinValue;

                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i] < minValue) minValue = values[i];
                    if (values[i] > maxValue) maxValue = values[i];
                }

                List<double> barValues = new List<double>();
                for (int i = 0; i < long.Min(maxBinCount, maxValue - minValue + 1); i++)
                {
                    barValues.Add(0);
                }

                double MaxBarValues = 0;
                for (int i = 0; i < values.Count; i++)
                {
                    barValues[(int)double.Min(barValues.Count - 1, (values[i] - minValue) / (double)double.Max(1d, maxValue - minValue) * barValues.Count)] += 1;
                }
                for (int i = 0; i < barValues.Count; i++)
                {
                    if (barValues[i] > MaxBarValues) MaxBarValues = barValues[i];
                }
                for (int i = 0; i < barValues.Count; i++)
                {
                    barValues[i] /= double.Max(0.000001d, MaxBarValues);
                }

                DrawHistogram(graphViewImage, barValues);

                return new RenderedGraphInformation(minValue, maxValue, GraphValueType.Long, 0, MaxBarValues, GraphValueType.Long);
            }
            else return new RenderedGraphInformation(0, 0, GraphValueType.Long, 0, 0, GraphValueType.Long);
        }

        public static RenderedGraphInformation DrawHistogram(Bitmap graphViewImage, List<double> values, int binCount)
        {
            if (values.Count > 0)
            {
                double minValue = double.MaxValue;
                double maxValue = double.MinValue;

                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i] < minValue) minValue = values[i];
                    if (values[i] > maxValue) maxValue = values[i];
                }

                List<double> barValues = new List<double>();
                for (int i = 0; i < binCount; i++)
                {
                    barValues.Add(0);
                }

                double MaxBarValues = 0;
                for (int i = 0; i < values.Count; i++)
                {
                    barValues[(int)double.Max(0, double.Min(barValues.Count - 1, (values[i] - minValue) / double.Max(0.000001d, maxValue - minValue) * barValues.Count))] += 1;
                }
                for (int i = 0; i < barValues.Count; i++)
                {
                    if (barValues[i] > MaxBarValues) MaxBarValues = barValues[i];
                }
                for (int i = 0; i < barValues.Count; i++)
                {
                    barValues[i] /= double.Max(0.000001d, MaxBarValues);
                }

                DrawHistogram(graphViewImage, barValues);

                return new RenderedGraphInformation(minValue, maxValue, GraphValueType.Double, 0, MaxBarValues, GraphValueType.Long);
            }
            else return new RenderedGraphInformation(0, 0, GraphValueType.Double, 0, 0, GraphValueType.Long);
        }

        public class RenderedGraphInformation
        {
            public double XMin;
            public double XMax;
            public GraphValueType XValueType;
            public double YMin;
            public double YMax;
            public GraphValueType YValueType;

            public RenderedGraphInformation() { }
            public RenderedGraphInformation(double xMin, double xMax, GraphValueType xValueType, double yMin, double yMax, GraphValueType yValueType)
            {
                XMin = xMin;
                XMax = xMax;
                XValueType = xValueType;
                YMin = yMin;
                YMax = yMax;
                YValueType = yValueType;
            }
        }

        public enum GraphValueType
        {
            Long,
            Double
        }
    }
}
