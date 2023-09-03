using StonkBotChartoMatic.ChartoMatic.Extensions;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace StonkBotChartoMatic.ChartoMatic
{
    class DbConn
    {
        private SQLiteConnection sqlite;

        public DbConn(string localDbPath, string networkDbPath, RunMode runMode)
        {
            try
            {
                switch (runMode)
                {
                    case RunMode.Local:
                        sqlite = new SQLiteConnection($"Data Source={localDbPath}");
                        break;
                    case RunMode.Remote:
                        string date = DateTime.Now.ToString("MM/dd/yyyy").Replace("/","-");
                        string tempDbPath = $"{Path.GetTempPath()}\\{date}_StonkBot.db".Replace("\\\\", "\\");
                        
                        if (File.Exists(tempDbPath) && File.Exists(networkDbPath))
                        {
                            DialogResult dialogResult = MessageBox.Show($"A database sync already occured today. Update with current DB data?", "Update local data?", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                File.Copy(networkDbPath, tempDbPath, true);
                            }

                            sqlite = new SQLiteConnection($"Data Source={tempDbPath}");
                            break;
                        }

                        if (File.Exists(networkDbPath))
                        {
                            MessageBox.Show("The remote DB will now be synced to this machine. This may take a bit - be PATIENT Dan.");
                            File.Copy(networkDbPath, tempDbPath, true);
                            sqlite = new SQLiteConnection($"Data Source={tempDbPath}");
                            break;
                        }
                        throw new Exception("RunMode: Remote - But unable to find DB at networkPath?  We should never see this.");
                    default:
                        throw new Exception("Unable to find DB and determine runMode. If running away from home check that your VPN connection is active!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error establishing connection to database: {ex.Message}");
            }
        }

        public DataTable esCandleQuery(string selectedDate, Market selectedMarket)
        {
            string prevDay = DateTime.Parse(selectedDate).AddDays(-1).ToString("yyyy-MM-dd");
            string query = (selectedMarket == Market.Day) ? $"select * from {SBTable.es_candles} where charttime like '%{selectedDate}%'" : $"select * from {SBTable.es_candles} where charttime like '%{selectedDate}%' or charttime like '%{prevDay}%'";
            SQLiteDataAdapter adapter;
            var dataTable = new DataTable();
            try
            {
                SQLiteCommand command;
                sqlite.Open();
                command = sqlite.CreateCommand();
                command.CommandText = query;
                adapter = new SQLiteDataAdapter(command);
                adapter.Fill(dataTable);
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"Error querying database: {ex}");
            }
            sqlite.Close();

            // Clean up the days outside of our intended range
            try
            {
                for (int i = dataTable.Rows.Count - 1; i >= 0; i--)
                {
                    var rowDateTime = Convert.ToDateTime(dataTable.Rows[i]["charttime"]);
                    switch (selectedMarket)
                    {
                        case Market.Day: // selDate 9:30 - selDate 17:00
                            if (rowDateTime < DateTime.Parse($"{selectedDate} 09:30:00")) { dataTable.Rows[i].Delete(); }
                            if (rowDateTime > DateTime.Parse($"{selectedDate} 17:00:00")) { dataTable.Rows[i].Delete(); }
                            break;
                        case Market.Night: // prevDate 18:00 - selDate 9:29
                            if (rowDateTime < DateTime.Parse($"{prevDay} 18:00:00")) { dataTable.Rows[i].Delete(); }
                            if (rowDateTime > DateTime.Parse($"{selectedDate} 09:29:00")) { dataTable.Rows[i].Delete(); }
                            break;
                        case Market.Both: // prevDate 18:00 - selDate 17:00
                            if (rowDateTime < DateTime.Parse($"{prevDay} 18:00:00")) { dataTable.Rows[i].Delete(); }
                            if (rowDateTime > DateTime.Parse($"{selectedDate} 17:00:00")) { dataTable.Rows[i].Delete(); }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cleaning up dataTable for this query: {ex.Message}");
            }

            dataTable.AcceptChanges();
            return dataTable;
        }

        public DataTable historyESQuery(string endDate, int range)
        {
            // build query
            endDate = DateTime.Parse(endDate).ToString("MM/dd/yy");
            string query = $"select * from {SBTable.history_es} where date like '%{endDate}%'";
            for (int i = 1; i <= range; i++)
            {
                string thisDate = DateTime.Parse(endDate).AddDays(-i).ToString("MM/dd/yy");
                query += $" or date like '%{thisDate}%'";
            }

            SQLiteDataAdapter adapter;
            var dataTable = new DataTable();
            try
            {
                SQLiteCommand command;
                sqlite.Open();
                command = sqlite.CreateCommand();
                command.CommandText = query;
                adapter = new SQLiteDataAdapter(command);
                adapter.Fill(dataTable);
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"Error querying database: {ex}");
            }
            sqlite.Close();
            dataTable.AcceptChanges();
            return dataTable;
        }

        public DataTable targetZoneQuery(string endDateStr, int range)
        {
            // build query
            DateTime endDate = DateTime.Parse(endDateStr);
            var thisDay = endDate.DayOfWeek;
            if (thisDay == DayOfWeek.Saturday || thisDay == DayOfWeek.Sunday)
            {
                range++;
            }
            
            endDateStr = endDate.ToString("yyyy-MM-dd");

            string query = $"select * from {SBTable.es_candles} where charttime like '%{endDateStr}%'";
            for (int i = 1; i <= range; i++)
            {
                thisDay = endDate.AddDays(-i).DayOfWeek;
                if (thisDay == DayOfWeek.Saturday || thisDay == DayOfWeek.Sunday)
                {
                    range++;
                    continue;
                }
                string thisDateStr = endDate.AddDays(-i).ToString("yyyy-MM-dd");
                query += $" or charttime like '%{thisDateStr}%'";
            }

            // add to dataTable
            SQLiteDataAdapter adapter;
            var dataTable = new DataTable();
            try
            {
                SQLiteCommand command;
                sqlite.Open();
                command = sqlite.CreateCommand();
                command.CommandText = query;
                adapter = new SQLiteDataAdapter(command);
                adapter.Fill(dataTable);
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"Error querying database: {ex}");
            }
            sqlite.Close();

            // Clean up the days outside of our intended range
            try
            {
                for (int i = dataTable.Rows.Count - 1; i >= 0; i--)
                {
                    var rowDateTime = Convert.ToDateTime(dataTable.Rows[i]["charttime"]);

                    var testHour = rowDateTime.Hour;
                    var testMinute = rowDateTime.Minute;

                    if (rowDateTime.Hour != 9 && rowDateTime.Hour != 10 && rowDateTime.Hour != 15)
                    {
                        dataTable.Rows[i].Delete();
                    }
                    
                    if (rowDateTime.Hour == 9 && rowDateTime.Minute < 30)
                    {
                        dataTable.Rows[i].Delete();
                    }
                    if (rowDateTime.Hour == 10 && rowDateTime.Minute > 10)
                    {
                        dataTable.Rows[i].Delete();
                    }
                    if (rowDateTime.Hour == 15 && rowDateTime.Minute < 20)
                    {
                        dataTable.Rows[i].Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cleaning up dataTable for this query: {ex.Message}");
            }

            dataTable.AcceptChanges();
            return dataTable;
        }
    }
}