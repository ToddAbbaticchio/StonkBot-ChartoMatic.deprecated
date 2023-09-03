using StonkBotChartoMatic.ChartoMatic;
using StonkBotChartoMatic.ChartoMatic.Extensions;
using StonkBotChartoMatic.ChartoMatic.FileHandler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace StonkBotChartoMatic
{
    public partial class Form1 : Form
    {
        public string selectedDate = null;
        public string selectedMarket = null;
        public int selectedCandleLen = 1;
        public List<CalloutAnnotation> labelList = new List<CalloutAnnotation>();
        public DataTable dataTable;
        public string droppedFilePath;
        readonly ChartUtils chartUtils = new ChartUtils();
        readonly FileHandler fileHandler = new FileHandler();
        readonly ChartArea c1Area;
        readonly ChartArea c2Area;
        readonly string localDbPath = @"E:\projects\stonkBot\Data\StonkBot_Mark3.db";
        readonly string networkDbPath = "//VILESTYLE/stonkbot/StonkBot_Mark3.db";
        public RunMode runMode;
        readonly DbConn dbConn;
        public DroppedFileMode droppedFileMode;

        public Form1()
        {
            InitializeComponent();

            // Form settings
            AllowDrop = true;
            DragDrop += Form1_DragDrop;
            DragEnter += Form1_DragEnter;
            chart1.MouseWheel += Chart_MouseWheel;
            //chart1.SelectionRangeChanged += Match_SelectionRangeChanged;
            chart1.MouseClick += Chart_RightClick;

            c1Area = chart1.ChartAreas[0];
            c1Area.CursorX.IsUserEnabled = true;
            c1Area.CursorY.IsUserEnabled = true;
            c1Area.CursorX.IsUserSelectionEnabled = true;
            c1Area.CursorY.IsUserSelectionEnabled = true;
            c1Area.CursorX.LineColor = Color.Transparent;
            c1Area.CursorY.LineColor = Color.Transparent;
            c1Area.CursorX.SelectionColor = Color.PaleTurquoise;
            c1Area.CursorY.SelectionColor = Color.PaleTurquoise;
            c1Area.CursorX.Interval = 0;
            c1Area.CursorY.Interval = 0;
            c1Area.AxisX.MajorGrid.LineColor = Color.WhiteSmoke;
            c1Area.AxisY.MajorGrid.LineColor = Color.WhiteSmoke;
            c1Area.AxisX.ScaleView.Zoomable = true;
            c1Area.AxisY.ScaleView.Zoomable = true;
            c1Area.AxisX.ScaleView.SmallScrollSizeType = DateTimeIntervalType.Minutes;
            c1Area.AxisX.ScaleView.SmallScrollSize = 30;
            c1Area.AxisX.ScaleView.SmallScrollMinSizeType = DateTimeIntervalType.Minutes;
            c1Area.AxisX.ScaleView.SmallScrollMinSize = 1;

            c2Area = chart2.ChartAreas[0];
            c2Area.CursorX.IsUserEnabled = true;
            c2Area.CursorY.IsUserEnabled = true;
            c2Area.CursorX.IsUserSelectionEnabled = true;
            c2Area.CursorY.IsUserSelectionEnabled = true;
            c2Area.CursorX.LineColor = Color.Transparent;
            c2Area.CursorY.LineColor = Color.Transparent;
            c2Area.CursorX.SelectionColor = Color.PaleTurquoise;
            c2Area.CursorY.SelectionColor = Color.PaleTurquoise;
            c2Area.CursorX.Interval = 0;
            c2Area.CursorY.Interval = 0;
            c2Area.AxisX.MajorGrid.LineColor = Color.WhiteSmoke;
            c2Area.AxisY.MajorGrid.LineColor = Color.WhiteSmoke;
            c2Area.AxisX.ScaleView.Zoomable = true;
            c2Area.AxisY.ScaleView.Zoomable = true;
            c2Area.AxisX.ScaleView.SmallScrollSizeType = DateTimeIntervalType.Minutes;
            c2Area.AxisX.ScaleView.SmallScrollSize = 30;
            c2Area.AxisX.ScaleView.SmallScrollMinSizeType = DateTimeIntervalType.Minutes;
            c2Area.AxisX.ScaleView.SmallScrollMinSize = 1;

            chart1.AxisViewChanged += Chart1_AxisViewChanged;
            //chart2.AxisViewChanged += Chart2_AxisViewChanged;

            ChartTypeDrop.DataSource = Enum.GetValues(typeof(SBChart));
            MarketDrop.DataSource = Enum.GetValues(typeof(Market));
            DatePicker.Value = DateTime.Now;
            CandleDrop.Text = "1";

            runMode = File.Exists(localDbPath) ? RunMode.Local : RunMode.Remote;
            //runMode = RunMode.Remote;
            this.Text = $"StonkBot_ChartoMatic: {runMode}";

            dbConn = new DbConn(localDbPath, networkDbPath, runMode);
        }

        /*private void Match_SelectionRangeChanged(object? sender, CursorEventArgs e)
        {
            if (e.Axis.AxisName.ToString() == "X")
            {
                Axis xAxis = c1Area.AxisX;
                double xStart = Math.Max(xAxis.Minimum, 0);
                double xEnd = c2Area.AxisX.Maximum;
                Chart_MatchPosition(xStart, xEnd);
            }
        }*/


        #region Event Handlers
        private void Chart_RightClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset(1);
                chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset(1);
                chart2.ChartAreas[0].AxisX.ScaleView.ZoomReset(1);
                chart2.ChartAreas[0].AxisY.ScaleView.ZoomReset(1);
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                // Handle FileDrop data.
                if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    return;
                }
                else
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (files.Length > 1)
                    {
                        MessageBox.Show("One file at a time please!");
                        return;
                    }
                    if (Path.GetExtension(files[0]) != ".csv")
                    {
                        MessageBox.Show($"The provided file is a {Path.GetExtension(files[0])}, please provide a .csv");
                        return;
                    }

                    droppedFilePath = files[0];
                    string droppedFileName = Path.GetFileName(files[0]);
                    
                    DialogResult dialogResult;
                    if (droppedFileName.Contains("trade-station-history"))
                    {
                        droppedFileMode = DroppedFileMode.TradeStation;
                        Regex getDate = new Regex("(\\d{4}-\\d{2}-\\d{2})");
                        Match match = getDate.Match(droppedFileName);
                        if (match.Success)
                        {
                            var dateString = match.Groups[1].Captures[0].Value;
                            var fileDate = DateTime.Parse(dateString);
                            DatePicker.Value = fileDate;
                            dialogResult = MessageBox.Show($"Import data from: {droppedFilePath}?", "Import data from file?", MessageBoxButtons.YesNo);

                            if (dialogResult == DialogResult.Yes)
                            {
                                dataTable = fileHandler.ImportFromTradingView(droppedFilePath, selectedDate, (Market)MarketDrop.SelectedValue, dbConn);
                                labelList = chartUtils.DrawTradeStationCandleChartWithLabelList(dataTable, chart1, chart2, selectedCandleLen);
                                //labelList = chartUtils.DrawCandleChartWithLabelList(dataTable, chart1, chart2, selectedCandleLen);
                            }
                        }
                        return;
                    }
                    else
                    {
                        droppedFileMode = DroppedFileMode.TDAmeritrade;
                        string droppedFileDate = droppedFileName.Substring(0, 10);
                        DatePicker.Value = DateTime.Parse(droppedFileDate);
                        dialogResult = MessageBox.Show($"Import data from: {droppedFilePath}?", "Import data from file?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            dataTable = fileHandler.Import(droppedFilePath, selectedDate, (Market)MarketDrop.SelectedValue, dbConn);
                            labelList = chartUtils.DrawCandleChartWithLabelList(dataTable, chart1, chart2, selectedCandleLen);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.Message);
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        #endregion

        #region Chart Zooming
        private const float CZoomScale = 1.25f;
        private int FZoomLevel = 0;
        private void Chart_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                Axis xAxis = chart1.ChartAreas[0].AxisX;
                double xMin = xAxis.ScaleView.ViewMinimum;
                double xMax = xAxis.ScaleView.ViewMaximum;
                double xPixelPos = xAxis.PixelPositionToValue(e.Location.X);

                Axis yAxis = chart1.ChartAreas[0].AxisY;
                double yMin = yAxis.ScaleView.ViewMinimum;
                double yMax = yAxis.ScaleView.ViewMaximum;
                double yPixelPos = yAxis.PixelPositionToValue(e.Location.Y);

                Axis x2Axis = chart2.ChartAreas[0].AxisX;
                Axis y2Axis = chart2.ChartAreas[0].AxisY;

                if (e.Delta < 0 && FZoomLevel > 0)
                {
                    // Scrolled down, meaning zoom out
                    if (--FZoomLevel <= 0)
                    {
                        FZoomLevel = 0;
                        xAxis.ScaleView.ZoomReset();
                        yAxis.ScaleView.ZoomReset();
                        x2Axis.ScaleView.ZoomReset();
                        y2Axis.ScaleView.ZoomReset();
                    }
                    else
                    {
                        double xStartPos = Math.Max(xPixelPos - (xPixelPos - xMin) * CZoomScale, 0);
                        double xEndPos = Math.Min(xStartPos + (xMax - xMin) * CZoomScale, xAxis.Maximum);
                        xAxis.ScaleView.Zoom(xStartPos, xEndPos);
                        double yStartPos = Math.Max(yPixelPos - (yPixelPos - yMin) * CZoomScale, 0);
                        double yEndPos = Math.Min(yStartPos + (yMax - yMin) * CZoomScale, yAxis.Maximum);
                        yAxis.ScaleView.Zoom(yStartPos, yEndPos);

                        Chart_MatchPosition(xStartPos, xEndPos);
                    }
                }
                else if (e.Delta > 0)
                {
                    // Scrolled up, meaning zoom in
                    double xStartPos = Math.Max(xPixelPos - (xPixelPos - xMin) / CZoomScale, 0);
                    double xEndPos = Math.Min(xStartPos + (xMax - xMin) / CZoomScale, xAxis.Maximum);
                    xAxis.ScaleView.Zoom(xStartPos, xEndPos);
                    double yStartPos = Math.Max(yPixelPos - (yPixelPos - yMin) / CZoomScale, 0);
                    double yEndPos = Math.Min(yStartPos + (yMax - yMin) / CZoomScale, yAxis.Maximum);
                    yAxis.ScaleView.Zoom(yStartPos, yEndPos);
                    FZoomLevel++;

                    Chart_MatchPosition(xStartPos, xEndPos);
                }
            }
            catch { }
        }

        private void Chart_MatchPosition(double xStartPos, double xEndPos)
        {
            c2Area.AxisX.ScaleView.Zoom(xStartPos, xEndPos);
        }
        #endregion

        #region Tool Interaction Functions
        private void ChartTypeDrop_SelectedIndexChanged(object sender, EventArgs e)
        {
            chart1.Annotations.Clear();
            chart1.Series.Clear();
            chart2.Series.Clear();
            var x = this.Width - 16;
            var y = this.Height - 83;
            var c1_y = Convert.ToInt32(y * .75);
            var c2_y = Convert.ToInt32(y * .25);

            switch (ChartTypeDrop.SelectedValue)
            {
                case SBChart.ESCandle:
                    chart1.Size = new Size(x, c1_y);
                    chart2.Size = new Size(x, c2_y);
                    chart2.Location = new Point(0, c1_y - 7);

                    chart2.Show();
                    MarketDrop.Show();
                    CandleDrop.Show();
                    ShowLabels.Show();
                    TextField.Hide();
                    break;
                case SBChart.FluxValue:
                    chart1.Size = new Size(x, y);

                    MarketDrop.Hide();
                    CandleDrop.Hide();
                    ShowLabels.Hide();
                    chart2.Hide();
                    TextField.Hide();
                    break;
                case SBChart.ESHighLow:
                    chart1.Size = new Size(x, y);

                    MarketDrop.Hide();
                    CandleDrop.Hide();
                    ShowLabels.Hide();
                    chart2.Hide();
                    TextField.Hide();
                    break;
                case SBChart.TargetZone:
                    chart1.Size = new Size(x, y);
                    TextField.Text = "";
                    //TextField.Size = new Size(252, 26);
                    TextField.Location = new Point((x / 2) - 126, 0);

                    MarketDrop.Hide();
                    CandleDrop.Show();
                    ShowLabels.Hide();
                    chart2.Hide();
                    TextField.Show();
                    break;
            }
        }

        private void DatePicker_ValueChanged(object sender, EventArgs e)
        {
            selectedDate = DatePicker.Value.ToString("yyyy-MM-dd");
            if (selectedDate == DateTime.Now.ToString("yyyy-MM-dd"))
            {
                MarketDrop.Text = "Both";
            }
        }

        private void MarketDrop_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            //if (MarketDrop.Text == "Both") selectedMarket = null;
            //else selectedMarket = MarketDrop.Text;
        }

        private void CandleDrop_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedCandleLen = Convert.ToInt32(CandleDrop.Text);
        }

        private void ShowLabels_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowLabels.CheckState == CheckState.Checked)
            {
                if (labelList.Count > 0)
                {
                    chart1.Annotations.Clear();
                    foreach (Annotation label in labelList)
                    {
                        chart1.Annotations.Add(label);
                    }
                }
                else
                {
                    MessageBox.Show("You need to import a daily transaction report first!");
                    ShowLabels.CheckState = CheckState.Unchecked;
                }
            }
            else
            {
                if (labelList.Count > 0)
                {
                    chart1.Annotations.Clear();
                    switch (droppedFileMode)
                    {
                        case DroppedFileMode.TDAmeritrade:
                            labelList = chartUtils.DrawCandleChartWithLabelList(dataTable, chart1, chart2, selectedCandleLen);
                            break;
                        case DroppedFileMode.TradeStation:
                            labelList = chartUtils.DrawTradeStationCandleChartWithLabelList(dataTable, chart1, chart2, selectedCandleLen);
                            break;
                        default:
                            MessageBox.Show("How did you get here without a valid droppedFileMode?  Thats weird. Get Todd.");
                            break;
                    }
                }
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (selectedDate == null)
            {
                MessageBox.Show("You must pick a date before updating charts!");
                return;
            }

            DataTable dataTable;
            List<DataTableRowV1> candleList;
            //chart1.ResetAutoValues();

            switch (ChartTypeDrop.SelectedValue)
            {
                // Handling for ES Candle Charts
                case SBChart.ESCandle:
                    chart1.Annotations.Clear();
                    dataTable = dbConn.esCandleQuery(selectedDate, (Market)MarketDrop.SelectedValue);
                    candleList = CandleResizer.SetSize(dataTable, selectedCandleLen);

                    if (labelList.Count > 0 && !(String.IsNullOrEmpty(droppedFilePath)))
                    {
                        dataTable = fileHandler.Import(droppedFilePath, selectedDate, (Market)MarketDrop.SelectedValue, dbConn);
                        labelList = chartUtils.DrawCandleChartWithLabelList(dataTable, chart1, chart2, selectedCandleLen);
                        if (ShowLabels.CheckState == CheckState.Checked)
                        {
                            foreach (Annotation label in labelList)
                            {
                                chart1.Annotations.Add(label);
                            }
                        }
                        return;
                    }

                    chartUtils.DrawCandleChart(candleList, dataTable, chart1, chart2);
                    break;

                // Handling for FluxValue Charts
                case SBChart.FluxValue:
                    chart1.Annotations.Clear();
                    dataTable = dbConn.historyESQuery(selectedDate, 10);
                    chartUtils.DrawFluxValueChart(dataTable, chart1);
                    break;

                // Handling for ES High/Low Charts
                case SBChart.ESHighLow:
                    chart1.Annotations.Clear();
                    dataTable = dbConn.historyESQuery(selectedDate, 10);
                    chartUtils.DrawESHighLowChart(dataTable, chart1);
                    break;

                // Handling for multi day zone chart
                case SBChart.TargetZone:
                    chart1.Annotations.Clear();
                    dataTable = dbConn.targetZoneQuery(selectedDate, 3);
                    candleList = CandleResizer.SetSize(dataTable, selectedCandleLen);
                    chartUtils.DrawTargetZoneCandleChart(candleList, dataTable, chart1, TextField);
                    break;
            }
        }

        #endregion

        #region TestCode

        private void Chart1_AxisViewChanged(object sender, ViewEventArgs e)
        {
            if (e.Axis.AxisName.ToString() != "X")
            {
                return;
            }

            Double viewMin = c1Area.AxisX.ScaleView.ViewMinimum;
            Double viewMax = c1Area.AxisX.ScaleView.ViewMaximum;

            var min = new DataPoint(0, Double.MaxValue);
            var max = new DataPoint(0, Double.MinValue);

            DataPointCollection points = chart1.Series[0].Points;
            foreach (DataPoint point in points)
            {
                if (point.XValue >= viewMin && point.XValue <= viewMax)
                {
                    if (point.YValues[0] > max.YValues[0])
                    {
                        max = point;
                    }
                    if (point.YValues[0] < min.YValues[0])
                    {
                        min = point;
                    }
                }
                else if (point.XValue >= viewMax)
                {
                    break;
                }
            }

            //min.Label = "Min " + min.YValues[0];
            //max.Label = "Max " + max.YValues[0];

            //c2Area.AxisX.Minimum = Math.Floor(min.XValue);
            //c2Area.AxisX.Maximum = Math.Ceiling(max.XValue);
            //MessageBox.Show($"minX: {min.XValue} maxX: {max.XValue}");
            Chart_MatchPosition(min.XValue, max.XValue);
        }



        /*private void Chart1_PostPaint(object? sender, ChartPaintEventArgs e)
        {
            string areaName = "startValue";
            if (e.ChartElement.GetType() == typeof(ChartArea))
            {
                areaName = c1Area.Name;
            }

            if (min != null && max != null)
            {
                Graphics graph = e.ChartGraphics.Graphics;

                Single pixelXMin = Convert.ToSingle(e.ChartGraphics.GetPositionFromAxis(areaName, AxisName.X, min.XValue));
                Single pixelYMin = Convert.ToSingle(e.ChartGraphics.GetPositionFromAxis(areaName, AxisName.Y, min.YValues[0]));
                Single pixelXMax = Convert.ToSingle(e.ChartGraphics.GetPositionFromAxis(areaName, AxisName.X, max.XValue));
                Single pixelYMax = Convert.ToSingle(e.ChartGraphics.GetPositionFromAxis(areaName, AxisName.Y, max.YValues[0]));

                var pointMin = new Point(Convert.ToInt32(pixelXMin), Convert.ToInt32(pixelYMin));
                var pointMax = new Point(Convert.ToInt32(pixelXMax), Convert.ToInt32(pixelYMax));
                var pMin = e.ChartGraphics.GetAbsolutePoint(pointMin);
                var pMax = e.ChartGraphics.GetAbsolutePoint(pointMax);

                graph.DrawLine(new Pen(Color.Red, 1), pMin, pMax);
        }*/
        #endregion
    }
}