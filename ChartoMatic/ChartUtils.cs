using StonkBotChartoMatic.ChartoMatic.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace StonkBotChartoMatic.ChartoMatic
{
    public class ChartUtils
    {
        public void DrawCandleChart(List<DataTableRowV1> candleList, DataTable dataTable, Chart chart, Chart chart2)
        {
            #region ChartSetup
            var chartSize = new CandleChartSizeInfo(dataTable);
            chart.ChartAreas[0].AxisY.Minimum = chartSize.YMin;
            chart.ChartAreas[0].AxisY.LabelStyle.Format = "0";
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
            chart.Series.Clear();

            chart2.ChartAreas[0].AxisY.LabelStyle.Format = "0";
            chart2.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
            chart2.Series.Clear();

            var cSeries = new Series();
            cSeries.ChartArea = "ChartArea1";
            cSeries.ChartType = SeriesChartType.Candlestick;
            cSeries.XValueType = ChartValueType.DateTime;
            cSeries.YValueType = ChartValueType.Double;
            cSeries.YValuesPerPoint = 4;

            var vSeries = new Series();
            vSeries.ChartArea = "ChartArea1";
            vSeries.ChartType = SeriesChartType.Column;
            vSeries.XValueType = ChartValueType.DateTime;
            vSeries.YValueType = ChartValueType.Double;
            #endregion

            foreach (DataTableRowV1 candle in candleList)
            {
                object[] candleInfo = { candle.Low, candle.High, candle.Open, candle.Close };
                var thisC = cSeries.Points.AddXY(candle.ChartTime, candleInfo);
                var thisV = vSeries.Points.AddXY(candle.ChartTime, candle.Volume);

                // Set C and V dataPoint colors
                if (candle.Open > candle.Close)
                {
                    cSeries.Points[thisC].Color = Color.FromArgb(255, Color.OrangeRed);
                    cSeries.Points[thisC].BackSecondaryColor = Color.FromArgb(255, Color.OrangeRed);
                    vSeries.Points[thisV].Color = Color.FromArgb(255, Color.OrangeRed);
                }
                else
                {
                    cSeries.Points[thisC].Color = Color.FromArgb(255, Color.ForestGreen);
                    vSeries.Points[thisV].Color = Color.FromArgb(255, Color.ForestGreen);
                }

                // Set dataPoint tooltips ???
                //cNewPoint.LabelToolTip = $"Open: {open}{Environment.NewLine}Close: {close}{Environment.NewLine}Low: {low}{Environment.NewLine}High: {high}{Environment.NewLine}Time: {time}{Environment.NewLine}Volume: {volume}";
                //vNewPoint.LabelToolTip = $"Volume: {volume}{Environment.NewLine}Time: {time}";
            }

            //chart.DataBind();
            chart.Series.Add(cSeries);
            chart2.Series.Add(vSeries);
        }

        public List<CalloutAnnotation> DrawCandleChartWithLabelList(DataTable dataTable, Chart chart, Chart chart2, int candleLen)
        {
            #region ChartSetup
            var chartSize = new CandleChartSizeInfo(dataTable);
            chart.ChartAreas[0].AxisY.Minimum = chartSize.YMin;
            chart.ChartAreas[0].AxisY.LabelStyle.Format = "0";
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
            chart.Series.Clear();

            chart2.ChartAreas[0].AxisY.LabelStyle.Format = "0";
            chart2.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
            chart2.Series.Clear();

            var cSeries = new Series();
            cSeries.ChartArea = "ChartArea1";
            cSeries.ChartType = SeriesChartType.Candlestick;
            cSeries.YValuesPerPoint = 4;
            cSeries.XValueType = ChartValueType.DateTime;
            cSeries.YValueType = ChartValueType.Double;
            var vSeries = new Series();
            vSeries.ChartArea = "ChartArea1";
            vSeries.ChartType = SeriesChartType.Column;
            vSeries.XValueType = ChartValueType.DateTime;
            vSeries.YValueType = ChartValueType.Double;
            var tSeries = new Series();
            tSeries.ChartArea = "ChartArea1";
            tSeries.ChartType = SeriesChartType.Point;
            tSeries.XValueType = ChartValueType.DateTime;
            tSeries.YValueType = ChartValueType.Double;
            tSeries.MarkerSize = 9;
            tSeries.MarkerBorderColor = Color.Black;
            tSeries.MarkerBorderWidth = 1;
            #endregion

            var candleList = CandleResizer.SetSizeV2(dataTable, candleLen);

            List<CalloutAnnotation> labelList = new List<CalloutAnnotation>();
            foreach (DataTableRowV2 candle in candleList)
            {
                object[] candleInfo = { candle.Low, candle.High, candle.Open, candle.Close };
                var thisC = cSeries.Points.AddXY(candle.ChartTime, candleInfo);
                var thisV = vSeries.Points.AddXY(candle.ChartTime, candle.Volume);

                // Set C and V dataPoint colors
                if (candle.Open > candle.Close)
                {
                    cSeries.Points[thisC].Color = Color.FromArgb(255, Color.OrangeRed);
                    cSeries.Points[thisC].BackSecondaryColor = Color.FromArgb(255, Color.OrangeRed);
                    vSeries.Points[thisV].Color = Color.FromArgb(255, Color.OrangeRed);
                }
                else
                {
                    cSeries.Points[thisC].Color = Color.FromArgb(255, Color.ForestGreen);
                    vSeries.Points[thisV].Color = Color.FromArgb(255, Color.ForestGreen);
                }

                // Set dataPoint tooltips ???
                //cNewPoint.LabelToolTip = $"Open: {open}{Environment.NewLine}Close: {close}{Environment.NewLine}Low: {low}{Environment.NewLine}High: {high}{Environment.NewLine}Time: {time}{Environment.NewLine}Volume: {volume}";
                //vNewPoint.LabelToolTip = $"Volume: {volume}{Environment.NewLine}Time: {time}";

                // If we have a transaction, plot it
                if (candle.TPrice > 0)
                {
                    var thisT = tSeries.Points.AddXY(candle.ChartTime, candle.TPrice);
                    if (candle.TSide.ToLower() == "buy" && candle.TPosEffect.ToLower() == "to open")
                    {
                        tSeries.Points[thisT].Color = Color.Red;
                    }
                    if (candle.TSide.ToLower() == "sell" && candle.TPosEffect.ToLower() == "to open")
                    {
                        tSeries.Points[thisT].Color = Color.MediumBlue;
                    }
                    if (candle.TPosEffect.ToLower() == "to close")
                    {
                        tSeries.Points[thisT].Color = Color.DarkOrange;
                    }

                    var label = new CalloutAnnotation();
                    label.AnchorDataPoint = tSeries.Points[thisT];
                    //label.Text = $"{dtRow.ChartTime.ToString("HH:mm")}\n${dtRow.TPrice}";
                    label.Text = $"${candle.TPrice}";
                    label.ForeColor = Color.White;
                    label.BackColor = Color.Black;
                    label.AnchorOffsetY = 1;
                    label.AnchorOffsetX = 1;
                    label.Font = new Font("Arial", 8); ;
                    label.Height = 4;
                    label.Width = 6;
                    label.CalloutStyle = CalloutStyle.RoundedRectangle;
                    labelList.Add(label);
                    //chart.Annotations.Add(label);

                    //tSeries.Points[thisT].Label = $"{labelTime}{Environment.NewLine}{dtRow.TPrice}";
                    tSeries.Points[thisT].ToolTip = "{1}" + Environment.NewLine + "{2}";
                }
            }

            //chart.DataBind();
            chart.Series.Add(cSeries);
            chart.Series.Add(tSeries);
            chart2.Series.Add(vSeries);

            return labelList;
        }

        public List<CalloutAnnotation> DrawTradeStationCandleChartWithLabelList(DataTable dataTable, Chart chart, Chart chart2, int candleLen)
        {
            #region ChartSetup
            var chartSize = new CandleChartSizeInfo(dataTable);
            chart.ChartAreas[0].AxisY.Minimum = chartSize.YMin;
            chart.ChartAreas[0].AxisY.LabelStyle.Format = "0";
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
            chart.Series.Clear();

            chart2.ChartAreas[0].AxisY.LabelStyle.Format = "0";
            chart2.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
            chart2.Series.Clear();

            var cSeries = new Series();
            cSeries.ChartArea = "ChartArea1";
            cSeries.ChartType = SeriesChartType.Candlestick;
            cSeries.YValuesPerPoint = 4;
            cSeries.XValueType = ChartValueType.DateTime;
            cSeries.YValueType = ChartValueType.Double;
            var vSeries = new Series();
            vSeries.ChartArea = "ChartArea1";
            vSeries.ChartType = SeriesChartType.Column;
            vSeries.XValueType = ChartValueType.DateTime;
            vSeries.YValueType = ChartValueType.Double;
            var tSeries = new Series();
            tSeries.ChartArea = "ChartArea1";
            tSeries.ChartType = SeriesChartType.Point;
            tSeries.XValueType = ChartValueType.DateTime;
            tSeries.YValueType = ChartValueType.Double;
            tSeries.MarkerSize = 9;
            tSeries.MarkerBorderColor = Color.Black;
            tSeries.MarkerBorderWidth = 1;
            #endregion

            var candleList = CandleResizer.SetSizeV2(dataTable, candleLen);
            //var transactionList = candleList.Where(x => x.TPrice != null && x.TPrice > 0).ToList();

            List<CalloutAnnotation> labelList = new List<CalloutAnnotation>();
            foreach (var candle in candleList)
            {              
                object[] candleInfo = { candle.Low, candle.High, candle.Open, candle.Close };
                var thisC = cSeries.Points.AddXY(candle.ChartTime, candleInfo);
                var thisV = vSeries.Points.AddXY(candle.ChartTime, candle.Volume);

                // Set C and V dataPoint colors
                if (candle.Open > candle.Close)
                {
                    cSeries.Points[thisC].Color = Color.FromArgb(255, Color.OrangeRed);
                    cSeries.Points[thisC].BackSecondaryColor = Color.FromArgb(255, Color.OrangeRed);
                    vSeries.Points[thisV].Color = Color.FromArgb(255, Color.OrangeRed);
                }
                else
                {
                    cSeries.Points[thisC].Color = Color.FromArgb(255, Color.ForestGreen);
                    vSeries.Points[thisV].Color = Color.FromArgb(255, Color.ForestGreen);
                }

                // Set dataPoint tooltips ???
                //cNewPoint.LabelToolTip = $"Open: {open}{Environment.NewLine}Close: {close}{Environment.NewLine}Low: {low}{Environment.NewLine}High: {high}{Environment.NewLine}Time: {time}{Environment.NewLine}Volume: {volume}";
                //vNewPoint.LabelToolTip = $"Volume: {volume}{Environment.NewLine}Time: {time}";

                // If we have a transaction, plot it
                if (candle.TPrice > 0 && candle.TPrice != null)
                {
                    var thisT = tSeries.Points.AddXY(candle.ChartTime, candle.TPrice);
                    if (candle.TSide.ToLower() == "buy" && candle.TPosEffect.ToLower().Contains("toopen"))
                    {
                        tSeries.Points[thisT].Color = Color.Red;
                    }
                    if (candle.TSide.ToLower() == "sell" && candle.TPosEffect.ToLower().Contains("toopen"))
                    {
                        tSeries.Points[thisT].Color = Color.MediumBlue;
                    }
                    if (candle.TPosEffect.ToLower().Contains("toclose"))
                    {
                        tSeries.Points[thisT].Color = Color.DarkOrange;
                    }

                    var label = new CalloutAnnotation();
                    label.AnchorDataPoint = tSeries.Points[thisT];
                    //label.Text = $"{dtRow.ChartTime.ToString("HH:mm")}\n${dtRow.TPrice}";
                    label.Text = $"${candle.TPrice}";
                    label.ForeColor = Color.White;
                    label.BackColor = Color.Black;
                    //label.AnchorOffsetY = 1;
                    //label.AnchorOffsetX = 1;
                    label.Font = new Font("Arial", 8);
                    //label.Height = 4;
                    //label.Width = 6;
                    label.CalloutStyle = CalloutStyle.RoundedRectangle;
                    labelList.Add(label);
                    //chart.Annotations.Add(label);

                    //tSeries.Points[thisT].Label = $"{labelTime}{Environment.NewLine}{dtRow.TPrice}";
                    //tSeries.Points[thisT].ToolTip = "asdf" + Environment.NewLine + "asdf";
                }
            }

            foreach (DataPoint dp in tSeries.Points)
            {
                dp.ToolTip = dp.XValue.ToString("MM/dd/yy HH:mm") + Environment.NewLine + dp.YValues.FirstOrDefault().ToString();
            }

            //chart.DataBind();
            chart.Series.Add(cSeries);
            chart.Series.Add(tSeries);
            chart2.Series.Add(vSeries);

            return labelList;
        }

        public void DrawFluxValueChart(DataTable dataTable, Chart chart)
        {
            #region ChartSetup
            var chartSize = new FluxChartSizeInfo(dataTable);
            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.LabelStyle.Format = "0";
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "MM/dd/yy";
            chart.ChartAreas[0].AxisX.Interval = 1;
            chart.Series.Clear();

            var fluxSeries = new Series();
            fluxSeries.ChartArea = "ChartArea1";
            fluxSeries.ChartType = SeriesChartType.Column;
            fluxSeries.XValueType = ChartValueType.DateTime;
            fluxSeries.YValueType = ChartValueType.Double;
            fluxSeries.Label = "#VALY";
            #endregion

            foreach (DataRow row in dataTable.Rows)
            {
                if (String.IsNullOrEmpty(row["absflux"].ToString()))
                {
                    continue;
                }

                var rowInfo = new HistoryESRow(row);
                fluxSeries.Points.AddXY(rowInfo.date, rowInfo.absflux);
            }

            chart.Series.Add(fluxSeries);
        }

        public void DrawESHighLowChart(DataTable dataTable, Chart chart)
        {
            #region ChartSetup
            var chartSize = new ESHighLowSizeInfo(dataTable);
            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.LabelStyle.Format = "0";
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "MM/dd/yy";
            chart.ChartAreas[0].AxisX.Interval = 1;
            chart.Series.Clear();

            var cSeries = new Series();
            cSeries.ChartArea = "ChartArea1";
            cSeries.ChartType = SeriesChartType.Column;
            cSeries.XValueType = ChartValueType.DateTime;
            cSeries.YValueType = ChartValueType.Double;
            cSeries.Label = "#VALY";
            #endregion

            foreach (DataRow row in dataTable.Rows)
            {
                if (String.IsNullOrEmpty(row["abslow"].ToString()))
                {
                    continue;
                }

                var rowInfo = new HistoryESRow(row);
                cSeries.Points.AddXY(rowInfo.date, (rowInfo.abshigh - rowInfo.abslow));
            }

            chart.Series.Add(cSeries);
        }

        public void DrawTargetZoneCandleChart(List<DataTableRowV1> rowList, DataTable dataTable, Chart chart, Label textField)
        {
            
            #region ChartSetup
            var chartSize = new CandleChartSizeInfo(dataTable);
            chart.ChartAreas[0].AxisY.Minimum = chartSize.YMin;
            chart.ChartAreas[0].AxisY.LabelStyle.Format = "0";
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "MM-dd HH:mm";
            chart.Series.Clear();

            var cSeries = new Series();
            cSeries.ChartArea = "ChartArea1";
            cSeries.ChartType = SeriesChartType.Candlestick;
            cSeries.XValueType = ChartValueType.DateTime;
            cSeries.YValueType = ChartValueType.Double;
            cSeries.YValuesPerPoint = 4;

            var midLineSeries = new Series();
            midLineSeries.ChartArea = "ChartArea1";
            midLineSeries.ChartType = SeriesChartType.Point;
            midLineSeries.XValueType = ChartValueType.DateTime;
            midLineSeries.YValueType = ChartValueType.Double;
            #endregion

            // Get unique days from data
            var dayList = new List<int>();
            var dayAvgList = new List<double>();
            foreach (var row in rowList)
            {
                var thisDay = row.ChartTime.Value.Day;
                if (!dayList.Contains(thisDay))
                {
                    dayList.Add(thisDay);
                }
            }

            // Split each day into am/pm and process datapoints
            for (var i = 0; i < dayList.Count; i++)
            {
                var day = dayList[i];
                
                // Sort the AM/PM entries for this day into lists
                var amDayDataList = rowList.FindAll(
                    delegate (DataTableRowV1 row)
                    {
                        return row.ChartTime.Value.Day == day && row.ChartTime.Value.Hour < 12;
                    });
                var pmDayDataList = rowList.FindAll(
                    delegate (DataTableRowV1 row)
                    {
                        return row.ChartTime.Value.Day == day && row.ChartTime.Value.Hour > 12;
                    });

                TZChartBuilder(amDayDataList, (i == dayList.Count -1) ? true : false);
                TZChartBuilder(pmDayDataList, (i == dayList.Count - 1) ? true : false);
            }

            // get midValue average
            double dayAvgCalc = 0;
            var dayAvgCount = dayAvgList.Count;
            foreach (var value in dayAvgList)
            {
                dayAvgCalc += value;
            }
            dayAvgCalc = Math.Round(dayAvgCalc / dayAvgCount, 2);
            textField.BackColor = Color.Black;
            textField.ForeColor = Color.White;
            textField.Text = $"Cross-day AM average values for the {dayAvgCount} days shown:{Environment.NewLine}Low:{.8 * dayAvgCalc}  |  Actual: {dayAvgCalc}  |  High: {1.2 * dayAvgCalc}";
            
            //chart.DataBind();
            chart.Series.Add(cSeries);
            chart.Series.Add(midLineSeries);

            void TZChartBuilder(List<DataTableRowV1> list, bool lastDay)
            {
                // Calc the Midline for the entries
                double thisLow = -1;
                double thisHigh = -1;

                foreach (var entry in list)
                {
                    if (thisLow == -1 || thisLow > entry.Low)
                    {
                        thisLow = entry.Low;
                    }
                    if (thisHigh == -1 || thisHigh < entry.High)
                    {
                        thisHigh = entry.High;
                    }
                }
                var thisMidline = Math.Round(((thisHigh - thisLow) / 2 + thisLow), 2);
                var dayAvg = Math.Round((thisHigh - thisLow), 2);

                dayAvgList.Add(dayAvg);

                // Graph things
                foreach (var candle in list)
                {
                    object[] candleInfo = { candle.Low, candle.High, candle.Open, candle.Close };
                    var thisC = cSeries.Points.AddXY(candle.ChartTime, candleInfo);
                    var thisML = midLineSeries.Points.AddXY(candle.ChartTime, thisMidline);

                    // Set dataPoint colors
                    if (candle.Open > candle.Close)
                    {
                        cSeries.Points[thisC].Color = Color.FromArgb(255, Color.OrangeRed);
                        cSeries.Points[thisC].BackSecondaryColor = Color.FromArgb(255, Color.OrangeRed);
                    }
                    else
                    {
                        cSeries.Points[thisC].Color = Color.FromArgb(255, Color.ForestGreen);
                    }
                    midLineSeries.Points[thisML].Color = Color.FromArgb(255, Color.Black);

                    // if the last entry for this day, add annotation to midline
                    if (candle.ChartTime == list[(list.Count - 1)].ChartTime) AddAnnotation(chart, midLineSeries.Points[thisML], $"{thisMidline}");

                    // If the last day in the day list add all the extra lines
                    if (lastDay == true)
                    {
                        var graphUpperHigh = (int)(thisHigh + dayAvg);
                        var graphUpperML = (int)(thisMidline + dayAvg);
                        var graphHigh = (int)(thisHigh);
                        var graphLow = (int)(thisLow);
                        var graphLowerML = (int)(thisMidline - dayAvg);
                        var graphLowerLow = (int)(thisLow - dayAvg);
                        
                        var upperHighPoint = midLineSeries.Points.AddXY(candle.ChartTime, graphUpperHigh);
                        var upperMLPoint = midLineSeries.Points.AddXY(candle.ChartTime, graphUpperML);
                        var highPoint = midLineSeries.Points.AddXY(candle.ChartTime, graphHigh);
                        var lowPoint = midLineSeries.Points.AddXY(candle.ChartTime, graphLow);
                        var lowerMLPoint = midLineSeries.Points.AddXY(candle.ChartTime, graphLowerML);
                        var lowerLowPoint = midLineSeries.Points.AddXY(candle.ChartTime, graphLowerLow);

                        midLineSeries.Points[upperHighPoint].Color = Color.FromArgb(90, Color.ForestGreen);
                        midLineSeries.Points[upperMLPoint].Color = Color.FromArgb(90, Color.ForestGreen);
                        midLineSeries.Points[highPoint].Color = Color.FromArgb(90, Color.ForestGreen);
                        midLineSeries.Points[lowPoint].Color = Color.FromArgb(90, Color.OrangeRed);
                        midLineSeries.Points[lowerMLPoint].Color = Color.FromArgb(90, Color.OrangeRed);
                        midLineSeries.Points[lowerLowPoint].Color = Color.FromArgb(90, Color.OrangeRed);

                        // if the last entry for this day, add annotation to midline
                        if (candle.ChartTime == list[(list.Count - 1)].ChartTime)
                        {
                            AddAnnotation(chart, midLineSeries.Points[upperHighPoint], $"{graphUpperHigh}");
                            AddAnnotation(chart, midLineSeries.Points[upperMLPoint], $"{graphUpperML}");
                            AddAnnotation(chart, midLineSeries.Points[highPoint], $"{graphHigh}");
                            AddAnnotation(chart, midLineSeries.Points[lowPoint], $"{graphLow}");
                            AddAnnotation(chart, midLineSeries.Points[lowerMLPoint], $"{graphLowerML}");
                            AddAnnotation(chart, midLineSeries.Points[lowerLowPoint], $"{graphLowerLow}");
                        }
                    }
                }
            }
        }

        private void AddAnnotation(Chart chart, DataPoint dataPoint, string labelText)
        {
            var label = new CalloutAnnotation();
            label.AnchorDataPoint = dataPoint;
            //label.IsSizeAlwaysRelative = false;
            label.Text = labelText;
            label.ForeColor = Color.Black;
            label.BackColor = Color.White;
            label.AnchorOffsetY = 1;
            label.AnchorOffsetX = 1;
            label.Font = new Font("Arial", 8); ;
            label.Height = 3;
            label.Width = 6;
            label.CalloutStyle = CalloutStyle.SimpleLine;
            chart.Annotations.Add(label);
        }
    }
}