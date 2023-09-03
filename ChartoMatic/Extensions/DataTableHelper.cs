using System;
using System.Data;

namespace StonkBotChartoMatic.ChartoMatic.Extensions
{
    public class DataTableRowV1
    {
        public DateTime? ChartTime { get; set; }
        public double Low { get; set; }
        public double High { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }

        public DataTableRowV1(DataRow row)
        {
            ChartTime = Convert.ToDateTime(row["charttime"]);
            Low = Convert.ToDouble(row["low"]);
            High = Convert.ToDouble(row["high"]);
            Open = Convert.ToDouble(row["open"]);
            Close = Convert.ToDouble(row["close"]);
            Volume = Convert.ToDouble(row["volume"]);
        }
    }

    public class DataTableRowV2
    {
        public DateTime? ChartTime { get; set; }
        public double Low { get; set; }
        public double High { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
        public DateTime? TExecTime { get; set; }
        public double? TPrice { get; set; }
        public string TSide { get; set; }
        public string TPosEffect { get; set; }

        public DataTableRowV2(DataRow row)
        {
            // setup
            var chartTime = Convert.ToDateTime(row["charttime"]);
            if (row.IsNull("tExecTime")) TExecTime = null;
                else TExecTime = Convert.ToDateTime(row["tExecTime"]);
            if (row.IsNull("tPrice")) TPrice = null;
                else TPrice = Convert.ToDouble(row["tPrice"]);


            // apply values
            ChartTime = chartTime;
            Low = Convert.ToDouble(row["low"]);
            High = Convert.ToDouble(row["high"]);
            Open = Convert.ToDouble(row["open"]);
            Close = Convert.ToDouble(row["close"]);
            Volume = Convert.ToDouble(row["volume"]);
            TSide = row["tSide"].ToString();
            TPosEffect = row["tPosEffect"].ToString();
        }
    }

    public class DataTableRowTradeStation
    {
        public DateTime? ChartTime { get; set; }
        public double Low { get; set; }
        public double High { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
        public DateTime? TExecTime { get; set; }
        public double? TPrice { get; set; }
        public string TSide { get; set; }
        public string TPosEffect { get; set; }

        public DataTableRowTradeStation(DataRow row)
        {
            // setup
            var chartTime = Convert.ToDateTime(row["charttime"]);
            if (row.IsNull("tExecTime")) TExecTime = null;
            else TExecTime = Convert.ToDateTime(row["tExecTime"]);
            if (row.IsNull("tPrice")) TPrice = null;
            else TPrice = Convert.ToDouble(row["tPrice"]);


            // apply values
            ChartTime = chartTime;
            Low = Convert.ToDouble(row["low"]);
            High = Convert.ToDouble(row["high"]);
            Open = Convert.ToDouble(row["open"]);
            Close = Convert.ToDouble(row["close"]);
            Volume = Convert.ToDouble(row["volume"]);
            TSide = row["tSide"].ToString();
            TPosEffect = row["tPosEffect"].ToString();
        }
    }

    public class HistoryESRow
    {
        public double? open { get; set; }
        public double? close { get; set; }
        public double? low { get; set; }
        public double? high { get; set; }
        public double? volume { get; set; }
        public string market { get; set; }
        public DateTime? date { get; set; }
        public double? absflux { get; set; }
        public double? dayflux { get; set; }
        public double? nightflux { get; set; }
        public double? abslow { get; set; }
        public double? abshigh { get; set; }

        public HistoryESRow(DataRow row)
        {
            if (row.IsNull("open")) open = null; else open = Convert.ToDouble(row["open"]);
            if (row.IsNull("close")) close = null; else close = Convert.ToDouble(row["close"]);
            if (row.IsNull("low")) low = null; else low = Convert.ToDouble(row["low"]);
            if (row.IsNull("high")) high = null; else high = Convert.ToDouble(row["high"]);
            if (row.IsNull("volume")) volume = null; else volume = Convert.ToDouble(row["volume"]);
            market = row["market"].ToString();
            date = Convert.ToDateTime(row["date"]);
            if (row.IsNull("absflux")) absflux = null; else absflux = Convert.ToDouble(row["absflux"]);
            if (row.IsNull("dayflux")) dayflux = null; else dayflux = Convert.ToDouble(row["dayflux"]);
            if (row.IsNull("nightflux")) nightflux = null; else nightflux = Convert.ToDouble(row["nightflux"]);
            if (row.IsNull("abslow")) abslow = null; else abslow = Convert.ToDouble(row["abslow"]);
            if (row.IsNull("abshigh")) abshigh = null; else abshigh = Convert.ToDouble(row["abshigh"]);
        }
    }
}