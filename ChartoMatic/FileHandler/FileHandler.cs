using StonkBotChartoMatic.ChartoMatic.Extensions;
using StonkBotChartoMatic.ChartoMatic.FileHandler.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace StonkBotChartoMatic.ChartoMatic.FileHandler
{
    class FileHandler
    {
        public DataTable ImportFromTradingView(string filePath, string selectedDate, Market selectedMarket, DbConn dbConn)
        {
            // Read file (bypass filelock from main form) and trim cancelled orders
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var streamReader = new StreamReader(fileStream, Encoding.Default);
            var allData = streamReader.ReadToEnd().Replace("Â ", "");
            var lines = Regex.Split(allData, "\n").ToList();
            fileStream.Close();
            streamReader.Close();
            lines.RemoveAt(0);
            lines.RemoveAll(x => x.Contains("NQZ22"));
            lines.RemoveAll(x => string.IsNullOrEmpty(x));
            
            // Map csv lines to TradingViewTransaction objects
            List<TradingViewTransaction> transactionsList = lines.Select(x => new TradingViewTransaction(x)).ToList().OrderBy(x => x.OpenTime).ToList();
            transactionsList.RemoveAll(x => x.Status == "Cancelled");
            
            // Flatten list
            var flatList = new List<TradingViewTransaction>();
            foreach (var t in transactionsList)
            {
                var replicateCount = t.Qty;
                for (var i = 1; i <= replicateCount; i++)
                {
                    var toAdd = new TradingViewTransaction
                    {
                        Symbol = t.Symbol,
                        Side = t.Side,
                        Type = t.Type,
                        Qty = 1,
                        SplitNote = $"{i} of {replicateCount}",
                        FilledQty = t.FilledQty,
                        LimitPrice = t.LimitPrice,
                        StopPrice = t.StopPrice,
                        AvgFillPrice = t.AvgFillPrice,
                        Status = t.Status,
                        OpenTime = t.OpenTime,
                        CloseTime = t.CloseTime,
                        Duration = t.Duration,
                        CommissionFee = t.CommissionFee,
                        ExpirationDate = t.ExpirationDate,
                        OrderId = t.OrderId,
                        CustomType = t.CustomType,
                    };
                    flatList.Add(toAdd);
                }
            }

            List<TradingViewTransaction> esList = flatList.Where(x => !x.Symbol.Contains("MES")).ToList().OrderBy(x => x.OpenTime).ToList();
            List<TradingViewTransaction> mesList = flatList.Where(x => x.Symbol.Contains("MES")).ToList().OrderBy(x => x.OpenTime).ToList();

            // Match up buy/sells starting from point of origin
            var esCounter = 0;
            foreach (var t in esList)
            {
                switch (t.Side)
                {
                    case "Sell" when (esCounter == 0):
                        esCounter--;
                        t.CustomType = "SellToOpen";
                        continue;
                    case "Buy" when (esCounter == 0):
                        esCounter++;
                        t.CustomType = "BuyToOpen";
                        continue;
                    case "Sell" when (esCounter > 0):
                        esCounter--;
                        t.CustomType = "SellToClose";
                        continue;
                    case "Buy" when (esCounter > 0):
                        esCounter++;
                        t.CustomType = "BuyToOpen";
                        continue;
                    case "Sell" when (esCounter < 0):
                        esCounter--;
                        t.CustomType = "SellToOpen";
                        continue;
                    case "Buy" when (esCounter < 0):
                        esCounter++;
                        t.CustomType = "BuyToClose";
                        continue;
                }
            }
            var mesCounter = 0;
            foreach (var t in mesList)
            {
                switch (t.Side)
                {
                    case "Sell" when (mesCounter == 0):
                        mesCounter--;
                        t.CustomType = "SellToOpen";
                        continue;
                    case "Buy" when (mesCounter == 0):
                        mesCounter++;
                        t.CustomType = "BuyToOpen";
                        continue;
                    case "Sell" when (mesCounter >= 0):
                        mesCounter--;
                        t.CustomType = "SellToClose";
                        continue;
                    case "Buy" when (mesCounter >= 0):
                        mesCounter++;
                        t.CustomType = "BuyToOpen";
                        continue;
                    case "Sell" when (mesCounter <= 0):
                        mesCounter--;
                        t.CustomType = "SellToOpen";
                        continue;
                    case "Buy" when (mesCounter <= 0):
                        mesCounter++;
                        t.CustomType = "BuyToClose";
                        continue;
                }
            }

            // make file for lazybutt Dan
            try
            {
                var outputDataTransactionList = new List<List<TradingViewTransaction>>();
                outputDataTransactionList.Add(esList.Where(x => x.CloseTime.ToString("yyyy-MM-dd") == selectedDate).ToList());
                outputDataTransactionList.Add(mesList.Where(x => x.CloseTime.ToString("yyyy-MM-dd") == selectedDate).ToList());

                var outputFileData = new TradingViewOutputData(outputDataTransactionList);
                var fileName = $"Summary - {selectedDate}.csv";
                File.WriteAllLines($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{fileName}", outputFileData.FileLines.ToList(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error writing file: {ex.Message}");
            }

            var dataTableList = new List<TradingViewTransaction>();
            dataTableList.AddRange(esList.Where(x => x.CloseTime.ToString("yyyy-MM-dd") == selectedDate).ToList());
            dataTableList.AddRange(mesList.Where(x => x.CloseTime.ToString("yyyy-MM-dd") == selectedDate).ToList());

            // get dataTable
            var dataTable = dbConn.esCandleQuery(selectedDate, selectedMarket);
            var preChangeRows = dataTable.Rows.Count;

            // add transaction info columns
            dataTable.Columns.Add("tExecTime");
            dataTable.Columns.Add("tPrice");
            dataTable.Columns.Add("tSide");
            dataTable.Columns.Add("tPosEffect");

            // match 'em up and add them based on times
            foreach (var transaction in dataTableList)
            {
                var transactionTime = transaction.CloseTime.ToString("yyyy-MM-dd HH:mm");
                // Slap 'em into the dataTable
                try
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        var r = dataTable.Rows[i];
                        var dataTableRowTime = Convert.ToDateTime(r["charttime"]).ToString("yyyy-MM-dd HH:mm");
                        if (transactionTime == dataTableRowTime)
                        {
                            var price = new List<decimal?> { transaction.AvgFillPrice, transaction.LimitPrice, transaction.StopPrice }.Max();

                            r["tExecTime"] = transaction.CloseTime;
                            r["tPrice"] = price;
                            r["tSide"] = transaction.Side;
                            r["tPosEffect"] = transaction.CustomType;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error matching transactions to dataTable: {ex.Message}");
                }
            }

            var postChangeRows = dataTable.Rows.Count;

            dataTable.AcceptChanges();
            return dataTable;
        }

        public DataTable Import(string filePath, string selectedDate, Market selectedMarket, DbConn dbConn)
        {
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var streamReader = new StreamReader(fileStream, Encoding.Default);
            var allData = streamReader.ReadToEnd();
            var lines = allData.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            var transactions = new List<Transaction>();

            bool filledOrders = false;
            //for (int i = 0; i < lines.Count(); i++)
            for (int i = 0; i < lines.Length; i++)
            {
                var thisLine = lines[i];
                if (String.IsNullOrEmpty(thisLine)) continue;
                if (thisLine.Contains("Exec Time")) continue;
                if (thisLine.Contains("Filled Orders"))
                {
                    filledOrders = true;
                    continue;
                }
                if (thisLine.Contains("Cancelled Orders"))
                {
                    break;
                }
                if (filledOrders == true)
                {
                    var transaction = new Transaction(thisLine);
                    transactions.Add(transaction);
                }
            }
            fileStream.Close();
            streamReader.Close();

            // make file for lazybutt Dan
            try
            {
                var outputFileData = new OutputFileData(transactions);
                var fileName = $"Summary - {selectedDate}.csv";
                File.WriteAllLines($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{fileName}", outputFileData.FileLines.ToList(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error writing file: {ex.Message}");
            }

            // get dataTable
            DataTable dataTable;
            dataTable = dbConn.esCandleQuery(selectedDate, selectedMarket);

            // add transaction info columns
            dataTable.Columns.Add("tExecTime");
            dataTable.Columns.Add("tPrice");
            dataTable.Columns.Add("tSide");
            dataTable.Columns.Add("tPosEffect");

            // match 'em up and add them based on times
            foreach (var transaction in transactions)
            {
                // Slap 'em into the dataTable
                try
                {
                    var tDate = transaction.ExecTime;
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        var row = dataTable.Rows[i];
                        var rowDate = Convert.ToString(row["charttime"]);
                        if (tDate == rowDate)
                        {
                            dataTable.Rows[i]["tExecTime"] = transaction.ExecTime;
                            dataTable.Rows[i]["tPrice"] = transaction.Price;
                            dataTable.Rows[i]["tSide"] = transaction.Side;
                            dataTable.Rows[i]["tPosEffect"] = transaction.PosEffect;
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error matching transactions to dataTable: {ex.Message}");
                }
            }

            dataTable.AcceptChanges();
            return dataTable;
        }
    }
}