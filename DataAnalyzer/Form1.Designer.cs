namespace DataAnalyzer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ComboBox comboBoxPorts;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtData;
        private LiveCharts.WinForms.CartesianChart cartesianChart1;
        private LiveCharts.WinForms.CartesianChart cartesianChart2;
        private System.Windows.Forms.Button btnSaveData;
        private System.Windows.Forms.Button btnSaveCharts;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.comboBoxPorts = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtData = new System.Windows.Forms.TextBox();
            this.cartesianChart1 = new LiveCharts.WinForms.CartesianChart();
            this.cartesianChart2 = new LiveCharts.WinForms.CartesianChart();
            this.btnSaveData = new System.Windows.Forms.Button();
            this.btnSaveCharts = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBoxPorts
            // 
            this.comboBoxPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPorts.FormattingEnabled = true;
            this.comboBoxPorts.Location = new System.Drawing.Point(12, 10);
            this.comboBoxPorts.Name = "comboBoxPorts";
            this.comboBoxPorts.Size = new System.Drawing.Size(121, 21);
            this.comboBoxPorts.TabIndex = 0;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(139, 10);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(94, 23);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Подключить";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtData
            // 
            this.txtData.Location = new System.Drawing.Point(12, 50);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtData.Size = new System.Drawing.Size(300, 150);
            this.txtData.TabIndex = 2;
            // 
            // cartesianChart1
            // 
            this.cartesianChart1.Location = new System.Drawing.Point(12, 220);
            this.cartesianChart1.Name = "cartesianChart1";
            this.cartesianChart1.Size = new System.Drawing.Size(500, 250);
            this.cartesianChart1.TabIndex = 3;
            // 
            // cartesianChart2
            // 
            this.cartesianChart2.Location = new System.Drawing.Point(12, 480);
            this.cartesianChart2.Name = "cartesianChart2";
            this.cartesianChart2.Size = new System.Drawing.Size(500, 250);
            this.cartesianChart2.TabIndex = 4;
            // 
            // btnSaveData
            // 
            this.btnSaveData.Location = new System.Drawing.Point(239, 10);
            this.btnSaveData.Name = "btnSaveData";
            this.btnSaveData.Size = new System.Drawing.Size(116, 23);
            this.btnSaveData.TabIndex = 5;
            this.btnSaveData.Text = "Сохранить данные";
            this.btnSaveData.UseVisualStyleBackColor = true;
            this.btnSaveData.Click += new System.EventHandler(this.btnSaveData_Click);
            // 
            // btnSaveCharts
            // 
            this.btnSaveCharts.Location = new System.Drawing.Point(361, 10);
            this.btnSaveCharts.Name = "btnSaveCharts";
            this.btnSaveCharts.Size = new System.Drawing.Size(116, 23);
            this.btnSaveCharts.TabIndex = 6;
            this.btnSaveCharts.Text = "Сохранить графики";
            this.btnSaveCharts.UseVisualStyleBackColor = true;
            this.btnSaveCharts.Click += new System.EventHandler(this.btnSaveCharts_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(534, 741);
            this.Controls.Add(this.btnSaveCharts);
            this.Controls.Add(this.btnSaveData);
            this.Controls.Add(this.cartesianChart2);
            this.Controls.Add(this.cartesianChart1);
            this.Controls.Add(this.txtData);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.comboBoxPorts);
            this.Name = "Form1";
            this.Text = "TeleRC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}