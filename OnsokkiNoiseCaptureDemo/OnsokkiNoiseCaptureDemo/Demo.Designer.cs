namespace OnsokkiNoiseCaptureDemo
{
    partial class Demo
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.btn_start = new System.Windows.Forms.Button();
            this.btn_stop = new System.Windows.Forms.Button();
            this.btn_test_data = new System.Windows.Forms.Button();
            this.chart_noise1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lbl_noise = new System.Windows.Forms.Label();
            this.dgv_test_data = new System.Windows.Forms.DataGridView();
            this.lbl_test_parameters = new System.Windows.Forms.Label();
            this.panel_main = new System.Windows.Forms.Panel();
            this.panel_table = new System.Windows.Forms.Panel();
            this.panel_chart = new System.Windows.Forms.Panel();
            this.panel_controls = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.chart_noise1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_test_data)).BeginInit();
            this.panel_main.SuspendLayout();
            this.panel_table.SuspendLayout();
            this.panel_chart.SuspendLayout();
            this.panel_controls.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_start
            // 
            this.btn_start.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btn_start.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btn_start.Location = new System.Drawing.Point(15, 16);
            this.btn_start.Margin = new System.Windows.Forms.Padding(2);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(114, 43);
            this.btn_start.TabIndex = 272;
            this.btn_start.Text = "Start Test";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // btn_stop
            // 
            this.btn_stop.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btn_stop.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btn_stop.Location = new System.Drawing.Point(142, 16);
            this.btn_stop.Margin = new System.Windows.Forms.Padding(2);
            this.btn_stop.Name = "btn_stop";
            this.btn_stop.Size = new System.Drawing.Size(114, 43);
            this.btn_stop.TabIndex = 273;
            this.btn_stop.Text = "Stop Test";
            this.btn_stop.UseVisualStyleBackColor = true;
            this.btn_stop.Click += new System.EventHandler(this.btn_stop_Click);
            // 
            // btn_test_data
            // 
            this.btn_test_data.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btn_test_data.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btn_test_data.Location = new System.Drawing.Point(270, 16);
            this.btn_test_data.Margin = new System.Windows.Forms.Padding(2);
            this.btn_test_data.Name = "btn_test_data";
            this.btn_test_data.Size = new System.Drawing.Size(114, 43);
            this.btn_test_data.TabIndex = 274;
            this.btn_test_data.Text = "Fetch Data & Generate Report";
            this.btn_test_data.UseVisualStyleBackColor = true;
            this.btn_test_data.Click += new System.EventHandler(this.btn_test_data_Click);
            // 
            // chart_noise1
            // 
            chartArea6.Name = "ChartArea1";
            this.chart_noise1.ChartAreas.Add(chartArea6);
            this.chart_noise1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend6.Name = "Legend1";
            this.chart_noise1.Legends.Add(legend6);
            this.chart_noise1.Location = new System.Drawing.Point(8, 8);
            this.chart_noise1.Margin = new System.Windows.Forms.Padding(8);
            this.chart_noise1.Name = "chart_noise1";
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series6.Legend = "Legend1";
            series6.Name = "Noise Level (dB)";
            this.chart_noise1.Series.Add(series6);
            this.chart_noise1.Size = new System.Drawing.Size(434, 677);
            this.chart_noise1.TabIndex = 275;
            this.chart_noise1.Text = "Noise Level Chart";
            // 
            // lbl_noise
            // 
            this.lbl_noise.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lbl_noise.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_noise.ForeColor = System.Drawing.Color.DarkBlue;
            this.lbl_noise.Location = new System.Drawing.Point(8, 685);
            this.lbl_noise.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_noise.Name = "lbl_noise";
            this.lbl_noise.Size = new System.Drawing.Size(434, 32);
            this.lbl_noise.TabIndex = 276;
            this.lbl_noise.Text = "Noise Value: --";
            this.lbl_noise.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgv_test_data
            // 
            this.dgv_test_data.AllowUserToAddRows = false;
            this.dgv_test_data.AllowUserToDeleteRows = false;
            this.dgv_test_data.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv_test_data.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_test_data.Location = new System.Drawing.Point(8, 32);
            this.dgv_test_data.Margin = new System.Windows.Forms.Padding(2);
            this.dgv_test_data.Name = "dgv_test_data";
            this.dgv_test_data.RowHeadersWidth = 51;
            this.dgv_test_data.RowTemplate.Height = 24;
            this.dgv_test_data.Size = new System.Drawing.Size(978, 653);
            this.dgv_test_data.TabIndex = 277;
            // 
            // lbl_test_parameters
            // 
            this.lbl_test_parameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_test_parameters.BackColor = System.Drawing.Color.LightBlue;
            this.lbl_test_parameters.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_test_parameters.Location = new System.Drawing.Point(8, 8);
            this.lbl_test_parameters.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_test_parameters.Name = "lbl_test_parameters";
            this.lbl_test_parameters.Size = new System.Drawing.Size(980, 24);
            this.lbl_test_parameters.TabIndex = 278;
            this.lbl_test_parameters.Text = "Test Parameters";
            this.lbl_test_parameters.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_test_parameters.Click += new System.EventHandler(this.lbl_test_parameters_Click);
            // 
            // panel_main
            // 
            this.panel_main.Controls.Add(this.panel_table);
            this.panel_main.Controls.Add(this.panel_chart);
            this.panel_main.Controls.Add(this.panel_controls);
            this.panel_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_main.Location = new System.Drawing.Point(0, 0);
            this.panel_main.Margin = new System.Windows.Forms.Padding(2);
            this.panel_main.Name = "panel_main";
            this.panel_main.Size = new System.Drawing.Size(1446, 798);
            this.panel_main.TabIndex = 279;
            // 
            // panel_table
            // 
            this.panel_table.Controls.Add(this.dgv_test_data);
            this.panel_table.Controls.Add(this.lbl_test_parameters);
            this.panel_table.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_table.Location = new System.Drawing.Point(0, 73);
            this.panel_table.Margin = new System.Windows.Forms.Padding(2);
            this.panel_table.Name = "panel_table";
            this.panel_table.Padding = new System.Windows.Forms.Padding(8);
            this.panel_table.Size = new System.Drawing.Size(996, 725);
            this.panel_table.TabIndex = 282;
            // 
            // panel_chart
            // 
            this.panel_chart.Controls.Add(this.chart_noise1);
            this.panel_chart.Controls.Add(this.lbl_noise);
            this.panel_chart.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel_chart.Location = new System.Drawing.Point(996, 73);
            this.panel_chart.Margin = new System.Windows.Forms.Padding(2);
            this.panel_chart.Name = "panel_chart";
            this.panel_chart.Padding = new System.Windows.Forms.Padding(8);
            this.panel_chart.Size = new System.Drawing.Size(450, 725);
            this.panel_chart.TabIndex = 281;
            // 
            // panel_controls
            // 
            this.panel_controls.BackColor = System.Drawing.Color.LightGray;
            this.panel_controls.Controls.Add(this.btn_start);
            this.panel_controls.Controls.Add(this.btn_stop);
            this.panel_controls.Controls.Add(this.btn_test_data);
            this.panel_controls.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_controls.Location = new System.Drawing.Point(0, 0);
            this.panel_controls.Margin = new System.Windows.Forms.Padding(2);
            this.panel_controls.Name = "panel_controls";
            this.panel_controls.Size = new System.Drawing.Size(1446, 73);
            this.panel_controls.TabIndex = 280;
            // 
            // Demo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1446, 798);
            this.Controls.Add(this.panel_main);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(754, 495);
            this.Name = "Demo";
            this.Text = "Onsokki Noise Capture Demo - Test Data Management";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Demo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart_noise1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_test_data)).EndInit();
            this.panel_main.ResumeLayout(false);
            this.panel_table.ResumeLayout(false);
            this.panel_chart.ResumeLayout(false);
            this.panel_controls.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.Button btn_stop;
        private System.Windows.Forms.Button btn_test_data;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_noise1;
        private System.Windows.Forms.Label lbl_noise;
        private System.Windows.Forms.DataGridView dgv_test_data;
        private System.Windows.Forms.Label lbl_test_parameters;
        private System.Windows.Forms.Panel panel_main;
        private System.Windows.Forms.Panel panel_controls;
        private System.Windows.Forms.Panel panel_chart;
        private System.Windows.Forms.Panel panel_table;
    }
}