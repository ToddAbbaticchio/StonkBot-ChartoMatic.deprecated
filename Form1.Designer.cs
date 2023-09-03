using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace StonkBotChartoMatic
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private Chart chart1;
        private Chart chart2;

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea11 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series11 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea12 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series12 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.DatePicker = new System.Windows.Forms.DateTimePicker();
            this.CandleDrop = new System.Windows.Forms.ComboBox();
            this.MarketDrop = new System.Windows.Forms.ComboBox();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.ShowLabels = new System.Windows.Forms.CheckBox();
            this.ChartTypeDrop = new System.Windows.Forms.ComboBox();
            this.TextField = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart1.BorderlineColor = System.Drawing.Color.Transparent;
            chartArea11.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea11);
            this.chart1.Location = new System.Drawing.Point(0, 0);
            this.chart1.Name = "chart1";
            series11.ChartArea = "ChartArea1";
            series11.Name = "Series1";
            this.chart1.Series.Add(series11);
            this.chart1.Size = new System.Drawing.Size(944, 347);
            this.chart1.TabIndex = 0;
            // 
            // chart2
            // 
            this.chart2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart2.BorderlineColor = System.Drawing.Color.Transparent;
            chartArea12.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea12);
            this.chart2.Location = new System.Drawing.Point(0, 340);
            this.chart2.Name = "chart2";
            series12.ChartArea = "ChartArea1";
            series12.Name = "Series1";
            this.chart2.Series.Add(series12);
            this.chart2.Size = new System.Drawing.Size(944, 117);
            this.chart2.TabIndex = 1;
            // 
            // DatePicker
            // 
            this.DatePicker.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.DatePicker.Location = new System.Drawing.Point(130, 475);
            this.DatePicker.Name = "DatePicker";
            this.DatePicker.Size = new System.Drawing.Size(200, 20);
            this.DatePicker.TabIndex = 2;
            this.DatePicker.ValueChanged += new System.EventHandler(this.DatePicker_ValueChanged);
            // 
            // CandleDrop
            // 
            this.CandleDrop.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.CandleDrop.Items.AddRange(new object[] {
            "1",
            "2",
            "5"});
            this.CandleDrop.Location = new System.Drawing.Point(463, 475);
            this.CandleDrop.Name = "CandleDrop";
            this.CandleDrop.Size = new System.Drawing.Size(100, 21);
            this.CandleDrop.TabIndex = 0;
            this.CandleDrop.SelectedIndexChanged += new System.EventHandler(this.CandleDrop_SelectedIndexChanged);
            // 
            // MarketDrop
            // 
            this.MarketDrop.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.MarketDrop.FormattingEnabled = true;
            this.MarketDrop.Items.AddRange(new object[] {
            "Both",
            "Day",
            "Night"});
            this.MarketDrop.Location = new System.Drawing.Point(357, 475);
            this.MarketDrop.Name = "MarketDrop";
            this.MarketDrop.Size = new System.Drawing.Size(100, 21);
            this.MarketDrop.TabIndex = 3;
            this.MarketDrop.SelectedIndexChanged += new System.EventHandler(this.MarketDrop_SelectedIndexChanged);
            // 
            // UpdateButton
            // 
            this.UpdateButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.UpdateButton.Location = new System.Drawing.Point(662, 475);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(75, 21);
            this.UpdateButton.TabIndex = 4;
            this.UpdateButton.Text = "Update!";
            this.UpdateButton.UseVisualStyleBackColor = true;
            this.UpdateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // ShowLabels
            // 
            this.ShowLabels.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.ShowLabels.AutoSize = true;
            this.ShowLabels.Location = new System.Drawing.Point(569, 478);
            this.ShowLabels.Name = "ShowLabels";
            this.ShowLabels.Size = new System.Drawing.Size(87, 17);
            this.ShowLabels.TabIndex = 5;
            this.ShowLabels.Text = "Show Labels";
            this.ShowLabels.UseVisualStyleBackColor = true;
            this.ShowLabels.CheckedChanged += new System.EventHandler(this.ShowLabels_CheckedChanged);
            // 
            // ChartTypeDrop
            // 
            this.ChartTypeDrop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ChartTypeDrop.Items.AddRange(new object[] {
            "ES Candle Charts",
            "Flux Value Charts",
            "More Charts",
            "Blahblah Charts"});
            this.ChartTypeDrop.Location = new System.Drawing.Point(745, 0);
            this.ChartTypeDrop.Name = "ChartTypeDrop";
            this.ChartTypeDrop.Size = new System.Drawing.Size(200, 21);
            this.ChartTypeDrop.TabIndex = 6;
            this.ChartTypeDrop.SelectedIndexChanged += new System.EventHandler(this.ChartTypeDrop_SelectedIndexChanged);
            // 
            // TextField
            // 
            this.TextField.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.TextField.AutoSize = true;
            this.TextField.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.TextField.Location = new System.Drawing.Point(0, 340);
            this.TextField.Name = "TextField";
            this.TextField.Size = new System.Drawing.Size(252, 13);
            this.TextField.TabIndex = 8;
            this.TextField.Text = "Cross-day AM average values for the 4 days shown:";
            this.TextField.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 501);
            this.Controls.Add(this.TextField);
            this.Controls.Add(this.ChartTypeDrop);
            this.Controls.Add(this.ShowLabels);
            this.Controls.Add(this.UpdateButton);
            this.Controls.Add(this.MarketDrop);
            this.Controls.Add(this.CandleDrop);
            this.Controls.Add(this.DatePicker);
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.chart1);
            this.MaximumSize = new System.Drawing.Size(3840, 2160);
            this.MinimumSize = new System.Drawing.Size(960, 540);
            this.Name = "Form1";
            this.Text = "StonkBot_ChartoMatic";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private DateTimePicker DatePicker;
        private ComboBox CandleDrop;
        private ComboBox MarketDrop;
        private Button UpdateButton;
        private CheckBox ShowLabels;
        private ComboBox ChartTypeDrop;
        private Label TextField;
    }
}