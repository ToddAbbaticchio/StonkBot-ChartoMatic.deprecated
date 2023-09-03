using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace StonkBotChartoMatic.ChartoMatic.Extensions
{
    internal class OutputFileData
    {
        public List<string> FileLines { get; set; }

        public OutputFileData(List<Transaction> transactions)
        {
            List<Transaction> t = transactions.OrderBy(x => x.ExecTime).ToList();
            var test = new List<string>();
            test.Add($"Symbol,Date,Day,Long or Short,Start Time,End Time,Time Frame,Enter Price,Close Price,Order Count,Close Count,Profit,Hold Time,Order Type,Reason(承接？追单？理由？技巧？市场价 还是limie order)");

            double profitSum = 0;
            while (t.Count > 1)
            {
                while (!t[0].PosEffect.Contains("TO OPEN") || !(t[0].Symbol.Contains("/ES") || t[0].Symbol.Contains("/MES")))
                {
                    t.Remove(t[0]);
                }
                var open = 0;
                var matchSymbol = t[0].Symbol;

                var close = 1;
                try
                {
                    while (!t[close].PosEffect.Contains("TO CLOSE") || t[close].Symbol != matchSymbol)
                    {
                        close++;
                    }

                    var o = new OutputEntry(t[open], t[close]);
                    test.Add($"{o.Symbol},{o.Date},{o.Day},{o.LongOrShort},{o.StartTime},{o.EndTime},{o.TimeFrame},{o.EnterPrice},{o.ClosePrice},{o.OrderCount},{o.CloseCount},{o.Profit},{o.HoldTime},{o.OrderType}");

                    profitSum += o.Profit;
                    //profitSum += (o.Symbol.Contains("ESM")) ? o.Profit * 5 : o.Profit * 50;
                    t.Remove(t[close]);
                    t.Remove(t[open]);
                }
                catch
                {
                    MessageBox.Show($"No 'to close' match found for position opened at: {t[open].ExecTime}");
                    t.Remove(t[open]);
                }
            }

            string message = (profitSum > 0) ? "»-(¯`·.·´¯)-> $$$ <-(¯`·.·´¯)-«" : "(╯°□°)╯︵ ┻━┻";
            test.Add($",,,,,,,,,,Total:,{profitSum},,{message},");
            FileLines = test;
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
        public string EnterPrice { get; set; }
        public string ClosePrice { get; set; }
        public string OrderCount { get; set; }
        public string CloseCount { get; set; }
        public double Profit { get; set; }
        public TimeSpan HoldTime { get; set; }
        public string OrderType { get; set; }

        public OutputEntry(Transaction tOpen, Transaction tClose)
        {
            var openTime = Convert.ToDateTime(tOpen.ExecTime);
            var closeTime = Convert.ToDateTime(tClose.ExecTime);

            Date = openTime.ToString("MM-dd-yyyy");
            Day = openTime.DayOfWeek.ToString();
            Symbol = tOpen.Symbol;
            LongOrShort = (tOpen.Side.ToString() == "BUY") ? "Long" : "Short";
            StartTime = openTime.ToString("HH:mm");
            EndTime = closeTime.ToString("HH:mm");
            EnterPrice = tOpen.Price;
            ClosePrice = tClose.Price;
            OrderCount = tOpen.Qty;
            CloseCount = tClose.Qty;
            Profit = (LongOrShort == "Long") ? Convert.ToDouble(tClose.Price) - Convert.ToDouble(tOpen.Price) : Convert.ToDouble(tOpen.Price) - Convert.ToDouble(tClose.Price);
            HoldTime = closeTime - openTime;
            OrderType = tOpen.OrderType;

            var h = openTime.TimeOfDay;
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
