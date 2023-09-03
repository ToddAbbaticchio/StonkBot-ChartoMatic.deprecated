using System;

namespace StonkBotChartoMatic.ChartoMatic.Extensions
{
    public class Transaction
    {
        public string ExecTime { get; set; }
        public string Spread { get; set; }
        public string Side { get; set; }
        public string Qty { get; set; }
        public string PosEffect { get; set; }
        public string Symbol { get; set; }
        public string Exp { get; set; }
        public string Strike { get; set; }
        public string Type { get; set; }
        public string Price { get; set; }
        public string NetPrice { get; set; }
        public string OrderType { get; set; }

        public Transaction(string csvLine)
        {
            // prep work
            string[] data = csvLine.Split(',');
            var formattedDate = Convert.ToDateTime(data[2]);

            // apply values
            ExecTime = formattedDate.ToString("yyyy-MM-dd HH:mm:00");
            Spread = data[3];
            Side = data[4];
            Qty = data[5];
            PosEffect = data[6];
            Symbol = data[7];
            Exp = data[8];
            Strike = data[9];
            Type = data[10];
            Price = data[11];
            NetPrice = data[12];
            OrderType = data[13];
        }
    }
}
