using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace StonkBotChartoMatic.ChartoMatic.FileHandler.Models
{
    internal class TradingViewOutputData
    {
        public List<string> FileLines { get; set; }

        public TradingViewOutputData(List<List<TradingViewTransaction>> listList)
        {
            var write = new List<string>();
            write.Add($"Symbol,Date,Day,Long or Short,Start Time,End Time,Time Frame,Enter Price,Close Price,Order Count,Close Count,Profit,Hold Time,Order Type,Reason(承接？追单？理由？技巧？市场价 还是limie order)");

            decimal? profitSum = 0;
            foreach (var list in listList)
            {
                while (list.Count > 2)
                {
                    // find the first 'toOpen' for the day
                    while (list[0].CustomType.Contains("ToClose"))
                    {
                        list.RemoveAt(0);
                    }

                    var openIndex = 0;
                    var findCloseIndex = 1;
                    var openType = list[openIndex].CustomType;
            
                    // find the next 'toClose'
                    switch (openType)
                    {
                        case "SellToOpen":
                            while (!list[findCloseIndex].CustomType.Contains("BuyToClose"))
                            {
                                findCloseIndex++;
                            }
                            break;
                        case "BuyToOpen":
                            while (!list[findCloseIndex].CustomType.Contains("SellToClose"))
                            {
                                findCloseIndex++;
                            }
                            break;
                    }

                    // write to file
                    var o = new OutputEntry(list[openIndex], list[findCloseIndex]);
                    write.Add($"{o.Symbol},{o.Date},{o.Day},{o.LongOrShort},{o.StartTime},{o.EndTime},{o.TimeFrame},{o.EnterPrice},{o.ClosePrice},{o.OrderCount},{o.CloseCount},{o.Profit},{o.HoldTime},{o.OrderType}");
                    profitSum += o.Profit;

                    list.RemoveAt(findCloseIndex);
                    list.RemoveAt(openIndex);

                }
            }
            
            string message = (profitSum > 0) ? "»-(¯`·.·´¯)-> $$$ <-(¯`·.·´¯)-«" : "(╯°□°)╯︵ ┻━┻";
            write.Add($",,,,,,,,,,Total:,{profitSum},,{message},");
            FileLines = write;
        }
    }

    internal class OutputEntry
    {
        public string Date { get; set; }
        public string Day { get; set; }
        public string Symbol { get; set; }
        public string LongOrShort { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string TimeFrame { get; set; }
        public decimal? EnterPrice { get; set; }
        public decimal? ClosePrice { get; set; }
        public int OrderCount { get; set; }
        public int CloseCount { get; set; }
        public decimal? Profit { get; set; }
        public TimeSpan HoldTime { get; set; }
        public string OrderType { get; set; }

        public OutputEntry(TradingViewTransaction tOpen, TradingViewTransaction tClose)
        {
            var enterPrice = new List<decimal?> { tOpen.LimitPrice, tOpen.AvgFillPrice }.Max();
            var closePrice = new List<decimal?> { tClose.LimitPrice, tClose.StopPrice, tClose.AvgFillPrice }.Max();
            var profitMod = (tOpen.Symbol.Contains("MES")) ? 5 : 50;

            Date = tOpen.OpenTime.ToString("MM-dd-yyyy");
            Day = tOpen.OpenTime.DayOfWeek.ToString();
            Symbol = tOpen.Symbol;
            LongOrShort = (tOpen.CustomType == "BuyToOpen") ? "Long" : "Short";
            StartTime = tOpen.OpenTime.ToString("HH:mm");
            EndTime = tClose.CloseTime.ToString("HH:mm");
            EnterPrice = enterPrice;
            ClosePrice = closePrice;
            OrderCount = tOpen.Qty;
            CloseCount = tClose.Qty;
            Profit = (LongOrShort == "Long") ? (ClosePrice - EnterPrice) * profitMod : (EnterPrice - ClosePrice) * profitMod;
            HoldTime = tClose.CloseTime - tOpen.OpenTime;
            OrderType = tOpen.Type;

            if (EnterPrice == 0 || ClosePrice == 0)
            {
                Console.WriteLine("stop here pls");
            }

            var h = tOpen.OpenTime.TimeOfDay;
            if (h >= TimeSpan.FromMinutes(1080) || h < TimeSpan.FromMinutes(120)) TimeFrame = "Asia";
            if (h >= TimeSpan.FromMinutes(120) && h < TimeSpan.FromMinutes(360)) TimeFrame = "Europe";
            if (h >= TimeSpan.FromMinutes(360) && h < TimeSpan.FromMinutes(570)) TimeFrame = "Early";
            if (h >= TimeSpan.FromMinutes(570) && h < TimeSpan.FromMinutes(630)) TimeFrame = "Open";
            if (h >= TimeSpan.FromMinutes(630) && h < TimeSpan.FromMinutes(900)) TimeFrame = "Mid";
            if (h >= TimeSpan.FromMinutes(900) && h < TimeSpan.FromMinutes(960)) TimeFrame = "Close";
            if (h >= TimeSpan.FromMinutes(960) && h < TimeSpan.FromMinutes(1020)) TimeFrame = "After";
        }
    }
}