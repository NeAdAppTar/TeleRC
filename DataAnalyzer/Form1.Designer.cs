using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Drawing;


namespace TeleRC
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtData;
        private LiveCharts.WinForms.CartesianChart cartesianChart1;
        private LiveCharts.WinForms.CartesianChart cartesianChart2;
        private PictureBox pictureBoxLidar;
        private System.Windows.Forms.TrackBar trackBarMove;
        private System.Windows.Forms.TrackBar trackBarTurn;
        private System.Windows.Forms.Label lblMoveValue;
        private System.Windows.Forms.Label lblTurnValue;
        private System.Windows.Forms.Timer portRefreshTimer;
        private string[] previousPorts = Array.Empty<string>();


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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txtData = new System.Windows.Forms.TextBox();
            this.cartesianChart1 = new LiveCharts.WinForms.CartesianChart();
            this.cartesianChart2 = new LiveCharts.WinForms.CartesianChart();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxLidar = new System.Windows.Forms.PictureBox();
            this.Описание = new System.Windows.Forms.ToolTip(this.components);
            this.rjButton2 = new CustomControls.RJControls.RJButton();
            this.rjButton1 = new CustomControls.RJControls.RJButton();
            this.txtSendData = new CustomControls.RJControls.RJTextBox();
            this.comboBoxPort = new RJComboBox();
            this.comboBoxBaudRate = new RJComboBox();
            this.btnConnectDisconnect = new CustomControls.RJControls.RJButton();
            this.btnClearCharts = new CustomControls.RJControls.RJButton();
            this.btnSendData = new CustomControls.RJControls.RJButton();
            this.btnSaveData = new CustomControls.RJControls.RJButton();
            this.btnSaveChart = new CustomControls.RJControls.RJButton();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLidar)).BeginInit();
            this.SuspendLayout();
            // 
            // txtData
            // 
            this.txtData.Font = new System.Drawing.Font("Moscow Sans Regular", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtData.Location = new System.Drawing.Point(1393, 13);
            this.txtData.Margin = new System.Windows.Forms.Padding(2);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.ReadOnly = true;
            this.txtData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtData.Size = new System.Drawing.Size(500, 503);
            this.txtData.TabIndex = 6;
            this.txtData.TextChanged += new System.EventHandler(this.txtData_TextChanged);
            // 
            // cartesianChart1
            // 
            this.cartesianChart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cartesianChart1.Location = new System.Drawing.Point(673, 2);
            this.cartesianChart1.Margin = new System.Windows.Forms.Padding(2);
            this.cartesianChart1.Name = "cartesianChart1";
            this.cartesianChart1.Size = new System.Drawing.Size(702, 891);
            this.cartesianChart1.TabIndex = 7;
            this.cartesianChart1.Text = "cartesianChart1";
            // 
            // cartesianChart2
            // 
            this.cartesianChart2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cartesianChart2.Location = new System.Drawing.Point(2, 2);
            this.cartesianChart2.Margin = new System.Windows.Forms.Padding(2);
            this.cartesianChart2.Name = "cartesianChart2";
            this.cartesianChart2.Size = new System.Drawing.Size(667, 891);
            this.cartesianChart2.TabIndex = 8;
            this.cartesianChart2.Text = "cartesianChart2";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.79852F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.20148F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 445F));
            this.tableLayoutPanel1.Controls.Add(this.cartesianChart1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cartesianChart2, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 144);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1377, 895);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // pictureBoxLidar
            // 
            this.pictureBoxLidar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxLidar.BackColor = System.Drawing.Color.Black;
            this.pictureBoxLidar.Location = new System.Drawing.Point(1393, 530);
            this.pictureBoxLidar.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBoxLidar.Name = "pictureBoxLidar";
            this.pictureBoxLidar.Size = new System.Drawing.Size(500, 500);
            this.pictureBoxLidar.TabIndex = 0;
            this.pictureBoxLidar.TabStop = false;
            this.pictureBoxLidar.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxLidar_Paint);
            // 
            // rjButton2
            // 
            this.rjButton2.BackColor = System.Drawing.Color.OrangeRed;
            this.rjButton2.BackgroundColor = System.Drawing.Color.OrangeRed;
            this.rjButton2.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rjButton2.BorderRadius = 10;
            this.rjButton2.BorderSize = 0;
            this.rjButton2.FlatAppearance.BorderSize = 0;
            this.rjButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rjButton2.Font = new System.Drawing.Font("Moscow Sans Regular", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rjButton2.ForeColor = System.Drawing.Color.White;
            this.rjButton2.Location = new System.Drawing.Point(984, 75);
            this.rjButton2.Margin = new System.Windows.Forms.Padding(2);
            this.rjButton2.Name = "rjButton2";
            this.rjButton2.Size = new System.Drawing.Size(129, 53);
            this.rjButton2.TabIndex = 23;
            this.rjButton2.Text = "Ровер Откл.";
            this.rjButton2.TextColor = System.Drawing.Color.White;
            this.rjButton2.UseVisualStyleBackColor = false;
            // 
            // rjButton1
            // 
            this.rjButton1.BackColor = System.Drawing.Color.OrangeRed;
            this.rjButton1.BackgroundColor = System.Drawing.Color.OrangeRed;
            this.rjButton1.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rjButton1.BorderRadius = 10;
            this.rjButton1.BorderSize = 0;
            this.rjButton1.FlatAppearance.BorderSize = 0;
            this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rjButton1.Font = new System.Drawing.Font("Moscow Sans Regular", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rjButton1.ForeColor = System.Drawing.Color.White;
            this.rjButton1.Location = new System.Drawing.Point(984, 10);
            this.rjButton1.Margin = new System.Windows.Forms.Padding(2);
            this.rjButton1.Name = "rjButton1";
            this.rjButton1.Size = new System.Drawing.Size(129, 53);
            this.rjButton1.TabIndex = 22;
            this.rjButton1.Text = "Ровер Вкл.";
            this.rjButton1.TextColor = System.Drawing.Color.White;
            this.rjButton1.UseVisualStyleBackColor = false;
            // 
            // txtSendData
            // 
            this.txtSendData.BackColor = System.Drawing.Color.OrangeRed;
            this.txtSendData.BorderColor = System.Drawing.Color.MediumSlateBlue;
            this.txtSendData.BorderFocusColor = System.Drawing.Color.HotPink;
            this.txtSendData.BorderRadius = 10;
            this.txtSendData.BorderSize = 1;
            this.txtSendData.Font = new System.Drawing.Font("Moscow Sans Regular", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtSendData.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtSendData.Location = new System.Drawing.Point(369, 10);
            this.txtSendData.Margin = new System.Windows.Forms.Padding(4);
            this.txtSendData.Multiline = false;
            this.txtSendData.Name = "txtSendData";
            this.txtSendData.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.txtSendData.PasswordChar = false;
            this.txtSendData.PlaceholderColor = System.Drawing.Color.DarkGray;
            this.txtSendData.PlaceholderText = "";
            this.txtSendData.Size = new System.Drawing.Size(200, 36);
            this.txtSendData.TabIndex = 21;
            this.txtSendData.Texts = "";
            this.txtSendData.UnderlinedStyle = false;
            this.txtSendData._TextChanged += new System.EventHandler(this.txtSendData__TextChanged);
            this.txtSendData.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSendData_KeyDown);
            // 
            // comboBoxPort
            // 
            this.comboBoxPort.BackColor = System.Drawing.Color.OrangeRed;
            this.comboBoxPort.BorderColor = System.Drawing.Color.MediumSlateBlue;
            this.comboBoxPort.BorderSize = 0;
            this.comboBoxPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.comboBoxPort.Font = new System.Drawing.Font("Moscow Sans Regular", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBoxPort.ForeColor = System.Drawing.Color.Transparent;
            this.comboBoxPort.IconColor = System.Drawing.Color.MediumSlateBlue;
            this.comboBoxPort.ListBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(228)))), ((int)(((byte)(245)))));
            this.comboBoxPort.ListTextColor = System.Drawing.Color.DimGray;
            this.comboBoxPort.Location = new System.Drawing.Point(10, 77);
            this.comboBoxPort.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxPort.MinimumSize = new System.Drawing.Size(100, 24);
            this.comboBoxPort.Name = "comboBoxPort";
            this.comboBoxPort.Size = new System.Drawing.Size(115, 51);
            this.comboBoxPort.TabIndex = 20;
            this.comboBoxPort.OnSelectedIndexChanged += new System.EventHandler(this.comboBoxPort_OnSelectedIndexChanged);
            // 
            // comboBoxBaudRate
            // 
            this.comboBoxBaudRate.AutoCompleteCustomSource.AddRange(new string[] {
            ""});
            this.comboBoxBaudRate.BackColor = System.Drawing.Color.OrangeRed;
            this.comboBoxBaudRate.BorderColor = System.Drawing.Color.MediumSlateBlue;
            this.comboBoxBaudRate.BorderSize = 0;
            this.comboBoxBaudRate.DisplayMember = "9600";
            this.comboBoxBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.comboBoxBaudRate.Font = new System.Drawing.Font("Moscow Sans Regular", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBoxBaudRate.ForeColor = System.Drawing.Color.Transparent;
            this.comboBoxBaudRate.IconColor = System.Drawing.Color.MediumSlateBlue;
            this.comboBoxBaudRate.Items.AddRange(new object[] {
            "9600",
            "14400",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.comboBoxBaudRate.ListBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(228)))), ((int)(((byte)(245)))));
            this.comboBoxBaudRate.ListTextColor = System.Drawing.Color.DimGray;
            this.comboBoxBaudRate.Location = new System.Drawing.Point(10, 10);
            this.comboBoxBaudRate.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxBaudRate.MinimumSize = new System.Drawing.Size(100, 24);
            this.comboBoxBaudRate.Name = "comboBoxBaudRate";
            this.comboBoxBaudRate.Size = new System.Drawing.Size(115, 55);
            this.comboBoxBaudRate.TabIndex = 19;
            this.comboBoxBaudRate.OnSelectedIndexChanged += new System.EventHandler(this.comboBoxBaudRate_OnSelectedIndexChanged);
            // 
            // btnConnectDisconnect
            // 
            this.btnConnectDisconnect.BackColor = System.Drawing.Color.OrangeRed;
            this.btnConnectDisconnect.BackgroundColor = System.Drawing.Color.OrangeRed;
            this.btnConnectDisconnect.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.btnConnectDisconnect.BorderRadius = 10;
            this.btnConnectDisconnect.BorderSize = 0;
            this.btnConnectDisconnect.FlatAppearance.BorderSize = 0;
            this.btnConnectDisconnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnectDisconnect.Font = new System.Drawing.Font("Moscow Sans Regular", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnConnectDisconnect.ForeColor = System.Drawing.Color.White;
            this.btnConnectDisconnect.Location = new System.Drawing.Point(137, 10);
            this.btnConnectDisconnect.Margin = new System.Windows.Forms.Padding(2);
            this.btnConnectDisconnect.Name = "btnConnectDisconnect";
            this.btnConnectDisconnect.Size = new System.Drawing.Size(187, 118);
            this.btnConnectDisconnect.TabIndex = 18;
            this.btnConnectDisconnect.Text = "Подключиться";
            this.btnConnectDisconnect.TextColor = System.Drawing.Color.White;
            this.btnConnectDisconnect.UseVisualStyleBackColor = false;
            this.btnConnectDisconnect.Click += new System.EventHandler(this.btnConnectDisconnect_Click);
            // 
            // btnClearCharts
            // 
            this.btnClearCharts.BackColor = System.Drawing.Color.OrangeRed;
            this.btnClearCharts.BackgroundColor = System.Drawing.Color.OrangeRed;
            this.btnClearCharts.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.btnClearCharts.BorderRadius = 10;
            this.btnClearCharts.BorderSize = 0;
            this.btnClearCharts.FlatAppearance.BorderSize = 0;
            this.btnClearCharts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearCharts.Font = new System.Drawing.Font("Moscow Sans Regular", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnClearCharts.ForeColor = System.Drawing.Color.White;
            this.btnClearCharts.Location = new System.Drawing.Point(673, 96);
            this.btnClearCharts.Margin = new System.Windows.Forms.Padding(2);
            this.btnClearCharts.Name = "btnClearCharts";
            this.btnClearCharts.Size = new System.Drawing.Size(190, 32);
            this.btnClearCharts.TabIndex = 17;
            this.btnClearCharts.Text = "Очистить графики";
            this.btnClearCharts.TextColor = System.Drawing.Color.White;
            this.btnClearCharts.UseVisualStyleBackColor = false;
            this.btnClearCharts.Click += new System.EventHandler(this.btnClearCharts_Click);
            // 
            // btnSendData
            // 
            this.btnSendData.BackColor = System.Drawing.Color.OrangeRed;
            this.btnSendData.BackgroundColor = System.Drawing.Color.OrangeRed;
            this.btnSendData.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.btnSendData.BorderRadius = 10;
            this.btnSendData.BorderSize = 0;
            this.btnSendData.FlatAppearance.BorderSize = 0;
            this.btnSendData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendData.Font = new System.Drawing.Font("Moscow Sans Regular", 15F);
            this.btnSendData.ForeColor = System.Drawing.Color.White;
            this.btnSendData.Location = new System.Drawing.Point(581, 10);
            this.btnSendData.Margin = new System.Windows.Forms.Padding(2);
            this.btnSendData.Name = "btnSendData";
            this.btnSendData.Size = new System.Drawing.Size(45, 36);
            this.btnSendData.TabIndex = 16;
            this.btnSendData.Text = " ↑";
            this.btnSendData.TextColor = System.Drawing.Color.White;
            this.btnSendData.UseVisualStyleBackColor = false;
            this.btnSendData.Click += new System.EventHandler(this.BtnSendData_Click);
            // 
            // btnSaveData
            // 
            this.btnSaveData.BackColor = System.Drawing.Color.OrangeRed;
            this.btnSaveData.BackgroundColor = System.Drawing.Color.OrangeRed;
            this.btnSaveData.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.btnSaveData.BorderRadius = 10;
            this.btnSaveData.BorderSize = 0;
            this.btnSaveData.FlatAppearance.BorderSize = 0;
            this.btnSaveData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveData.Font = new System.Drawing.Font("Moscow Sans Regular", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSaveData.ForeColor = System.Drawing.Color.White;
            this.btnSaveData.Location = new System.Drawing.Point(673, 11);
            this.btnSaveData.Margin = new System.Windows.Forms.Padding(2);
            this.btnSaveData.Name = "btnSaveData";
            this.btnSaveData.Size = new System.Drawing.Size(190, 32);
            this.btnSaveData.TabIndex = 15;
            this.btnSaveData.Text = "Сохранить данные";
            this.btnSaveData.TextColor = System.Drawing.Color.White;
            this.btnSaveData.UseVisualStyleBackColor = false;
            this.btnSaveData.Click += new System.EventHandler(this.btnSaveData_Click);
            // 
            // btnSaveChart
            // 
            this.btnSaveChart.BackColor = System.Drawing.Color.OrangeRed;
            this.btnSaveChart.BackgroundColor = System.Drawing.Color.OrangeRed;
            this.btnSaveChart.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.btnSaveChart.BorderRadius = 10;
            this.btnSaveChart.BorderSize = 0;
            this.btnSaveChart.FlatAppearance.BorderSize = 0;
            this.btnSaveChart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveChart.Font = new System.Drawing.Font("Moscow Sans Regular", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSaveChart.ForeColor = System.Drawing.Color.White;
            this.btnSaveChart.Location = new System.Drawing.Point(673, 54);
            this.btnSaveChart.Margin = new System.Windows.Forms.Padding(2);
            this.btnSaveChart.Name = "btnSaveChart";
            this.btnSaveChart.Size = new System.Drawing.Size(190, 32);
            this.btnSaveChart.TabIndex = 14;
            this.btnSaveChart.Text = "Сохранить график";
            this.btnSaveChart.TextColor = System.Drawing.Color.White;
            this.btnSaveChart.UseVisualStyleBackColor = false;
            this.btnSaveChart.Click += new System.EventHandler(this.btnSaveChart_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1904, 1041);
            this.Controls.Add(this.pictureBoxLidar);
            this.Controls.Add(this.rjButton2);
            this.Controls.Add(this.rjButton1);
            this.Controls.Add(this.txtSendData);
            this.Controls.Add(this.comboBoxPort);
            this.Controls.Add(this.comboBoxBaudRate);
            this.Controls.Add(this.btnConnectDisconnect);
            this.Controls.Add(this.btnClearCharts);
            this.Controls.Add(this.btnSendData);
            this.Controls.Add(this.btnSaveData);
            this.Controls.Add(this.btnSaveChart);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.txtData);
            this.Font = new System.Drawing.Font("Moscow Sans Regular", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "TeleRC";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLidar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private CustomControls.RJControls.RJButton btnSaveChart;
        private CustomControls.RJControls.RJButton btnSaveData;
        private CustomControls.RJControls.RJButton btnSendData;
        private CustomControls.RJControls.RJButton btnClearCharts;
        private CustomControls.RJControls.RJButton btnConnectDisconnect;
        private RJComboBox comboBoxBaudRate;
        private RJComboBox comboBoxPort;
        private CustomControls.RJControls.RJTextBox txtSendData;
        private CustomControls.RJControls.RJButton rjButton1;
        private CustomControls.RJControls.RJButton rjButton2;
        private ToolTip Описание;
    }
}