using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace StonkBotChartoMatic.ChartoMatic.Extensions
{
    public class CandleChartSizeInfo
    {
        public double XInterval { get; set; }
        public double YMin { get; set; }
        public double YMax { get; set; }
        public double YInterval { get; set; }
        public double VolumeMax { get; set; }

        public CandleChartSizeInfo(DataTable dataTable)
        {
            if (dataTable.Rows.Count == 0)
            {
                MessageBox.Show("The database doesn't contain any entries that match the requested date and market time(s).");
                return;
            }
            try
            {
                // prep work
                double yVal = Convert.ToDouble(dataTable.Compute("min([low])", ""));
                double VolMax = Convert.ToDouble(dataTable.Compute("max([volume])", ""));
                var YIntervals = new List<double> { 10, 25, 50, 100, 250, 500 };
                var YIntervalApprox = (YMax - YMin) / 100;

                // apply values
                YMin = 25 * Math.Floor(yVal / 25);
                YMax = Convert.ToDouble(dataTable.Compute("max([high])", ""));
                VolumeMax = VolMax;
                YInterval = YIntervals.OrderBy(x => Math.Abs(YIntervalApprox - x)).First();
                XInterval = 15;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating CandleChartSizeInfo object: {ex.Message}");
            }
        }
    }

    public class FluxChartSizeInfo
    {
        public double XInterval { get; set; }
        public double YMin { get; set; }
        public double YMax { get; set; }
        public double YInterval { get; set; }

        public FluxChartSizeInfo(DataTable dataTable)
        {
            if (dataTable.Rows.Count == 0)
            {
                MessageBox.Show("The database doesn't contain any entries that match the requested date and market time(s).");
                return;
            }
            try
            {
                // prep work
                var yMinMath = Convert.ToDouble(dataTable.Compute("min([absflux])", "")) - 10;
                var yMin = (yMinMath < 0) ? 0 : yMinMath;
                var YIntervals = new List<double> { 10, 25, 50, 100, 250, 500 };
                var YIntervalApprox = (YMax - YMin) / 100;

                // apply values

                YMin = yMin;
                YMax = Convert.ToDouble(dataTable.Compute("max([absflux])", ""));
                YInterval = YIntervals.OrderBy(x => Math.Abs(YIntervalApprox - x)).First();
                XInterval = 1;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating FluxChartSizeInfo object: {ex.Message}");
            }
        }
    }

    public class ESHighLowSizeInfo
    {
        public double XInterval { get; set; }
        public double YMin { get; set; }
        public double YMax { get; set; }
        public double YInterval { get; set; }

        public ESHighLowSizeInfo(DataTable dataTable)
        {
            if (dataTable.Rows.Count == 0)
            {
                MessageBox.Show("The database doesn't contain any entries that match the requested date and market time(s).");
                return;
            }
            try
            {
                // prep work
                var abslow = Convert.ToDouble(dataTable.Compute("min([abslow])", ""));
                var abshigh = Convert.ToDouble(dataTable.Compute("min([abshigh])", ""));
                var YIntervals = new List<double> { 10, 25, 50, 100, 250, 500 };
                var YIntervalApprox = (YMax - YMin) / 100;

                // apply values

                YMin = abslow;
                YMax = abshigh;
                YInterval = YIntervals.OrderBy(x => Math.Abs(YIntervalApprox - x)).First();
                XInterval = 1;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating FluxChartSizeInfo object: {ex.Message}");
            }
        }
    }
}