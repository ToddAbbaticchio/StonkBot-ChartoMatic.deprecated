using System;
using System.Text;
using System.Windows.Forms;

namespace StonkBotChartoMatic.ChartoMatic.FileHandler.Models
{
    public class TradingViewTransaction
    {
        public string Symbol { get; set; }
        public string Side { get; set; }
        public string Type { get; set; }
        public int Qty { get; set; }
        public string SplitNote { get; set; }
        public int FilledQty { get; set; }
        public decimal? LimitPrice { get; set; }
        public decimal? StopPrice { get; set; }
        public decimal? AvgFillPrice { get; set; }
        public string Status { get; set; }
        public DateTime OpenTime { get; set; }
        public DateTime CloseTime { get; set; }
        public string Duration { get; set; }
        public decimal? CommissionFee { get; set; }
        public string ExpirationDate { get; set; }
        public string OrderId { get; set; }
        public string CustomType { get; set; }

        public TradingViewTransaction() { }

        public TradingViewTransaction(string csvLine)
        {
            try
            {
                string[] data = csvLine.Split(',');

                Symbol = data[0];
                Side = data[1];
                Type = data[2];
                Qty = Convert.ToInt32(data[3]);
                SplitNote = "";
                FilledQty = Convert.ToInt32(data[4]);
                LimitPrice = (string.IsNullOrEmpty(data[5])) ? Convert.ToDecimal(0) : Convert.ToDecimal(data[5]);
                StopPrice = (string.IsNullOrEmpty(data[6])) ? Convert.ToDecimal(0) : Convert.ToDecimal(data[6]);
                AvgFillPrice = (string.IsNullOrEmpty(data[7])) ? Convert.ToDecimal(0) : Convert.ToDecimal(data[7]);
                Status = data[8];
                OpenTime = Convert.ToDateTime(data[9]);
                CloseTime = Convert.ToDateTime(data[10]);
                Duration = data[11];
                CommissionFee = (string.IsNullOrEmpty(data[12])) ? Convert.ToDecimal(0) : Convert.ToDecimal(data[12]);
                ExpirationDate = data[13];
                OrderId = data[14];
                CustomType = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing file: {ex.Message}");
            }
        }
    }
}
