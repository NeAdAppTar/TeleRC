using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.WinForms;
using LiveCharts.Wpf;

namespace TeleRC
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort;
        private bool isConnected = false;

        // Отдельные серии для каждой оси ускорения
        private ChartValues<double> accelXValues;
        private ChartValues<double> accelYValues;
        private ChartValues<double> accelZValues;
        private ChartValues<double> altitudeValues;

        public Form1()
        {
            InitializeComponent();
            LoadAvailablePorts();
            InitializeCharts();
        }

        private void LoadAvailablePorts()
        {
            string[] ports = SerialPort.GetPortNames();
            comboBoxPort.Items.AddRange(ports);
            if (ports.Length > 0)
            {
                comboBoxPort.SelectedIndex = 0;
            }
        }

        // Добавим новый набор данных для абсолютного ускорения
        private ChartValues<double> accelAbsValues;

        private void InitializeCharts()
        {
            altitudeValues = new ChartValues<double>();
            accelXValues = new ChartValues<double>();
            accelYValues = new ChartValues<double>();
            accelZValues = new ChartValues<double>();
            accelAbsValues = new ChartValues<double>();  // Новая линия для абсолютного ускорения

            // График высоты
            cartesianChart1.Series = new SeriesCollection
    {
        new LineSeries
        {
            Title = "Altitude",
            Values = altitudeValues,
            StrokeThickness = 2,
            PointGeometrySize = 5
        }
    };

            // График ускорений (три оси + абсолютное ускорение)
            cartesianChart2.Series = new SeriesCollection
    {
        new LineSeries
        {
            Title = "Acceleration X",
            Values = accelXValues,
            StrokeThickness = 2,
            PointGeometrySize = 5
        },
        new LineSeries
        {
            Title = "Acceleration Y",
            Values = accelYValues,
            StrokeThickness = 2,
            PointGeometrySize = 5
        },
        new LineSeries
        {
            Title = "Acceleration Z",
            Values = accelZValues,
            StrokeThickness = 2,
            PointGeometrySize = 5
        },
        new LineSeries
        {
            Title = "Acceleration Abs",
            Values = accelAbsValues,
            StrokeThickness = 2,
            PointGeometrySize = 5
        }
    };

            // Оси графика высоты
            cartesianChart1.AxisX.Add(new Axis
            {
                Title = "Time (s)",
                LabelFormatter = value => TimeSpan.FromSeconds(value).ToString(@"mm\:ss")
            });

            cartesianChart1.AxisY.Add(new Axis
            {
                Title = "Altitude (m)",
                LabelFormatter = value => value.ToString("0.##")
            });

            // Оси графика ускорений
            cartesianChart2.AxisX.Add(new Axis
            {
                Title = "Time (s)",
                LabelFormatter = value => TimeSpan.FromSeconds(value).ToString(@"mm\:ss")
            });

            cartesianChart2.AxisY.Add(new Axis
            {
                Title = "Acceleration (m/s²)",
                LabelFormatter = value => value.ToString("0.##")
            });
        }

        private void UpdateCharts(string data)
        {
            string[] values = data.Split(';');
            if (values.Length >= 6)
            {
                double altitude = Convert.ToDouble(values[2]) / 100.0;
                double accelX = Convert.ToDouble(values[3]) * 9.81 / 100.0;
                double accelY = Convert.ToDouble(values[4]) * 9.81 / 100.0;
                double accelZ = Convert.ToDouble(values[5]) * 9.81 / 100.0;

                // Абсолютное ускорение
                double accelAbs = Math.Sqrt(accelX * accelX + accelY * accelY + accelZ * accelZ);

                altitudeValues.Add(altitude);
                accelXValues.Add(accelX);
                accelYValues.Add(accelY);
                accelZValues.Add(accelZ);
                accelAbsValues.Add(accelAbs);
            }
        }

        private void btnConnectDisconnect_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                try
                {
                    
                    
                    string selectedPort = comboBoxPort.SelectedItem.ToString();
                    int baudRate = int.Parse(comboBoxBaudRate.SelectedItem.ToString());

                    serialPort = new SerialPort(selectedPort, baudRate);
                    serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                    serialPort.Open();
                    AddMessage("Подключено к порту " + selectedPort + " со скоростью " + baudRate);
                    btnConnectDisconnect.Text = "Отключиться";
                    isConnected = true;
                }
                catch (Exception ex)
                {
                    // AddMessage("Ошибка при подключении: " + ex.Message);
                    MessageBox.Show("[C1001] " + "Ошибка при подключении: " + ex.Message, "Ошибка", MessageBoxButtons.OK);
                }
            }
            else
            {
                try
                {
                    serialPort.Close();
                    AddMessage("Отключено.");
                    btnConnectDisconnect.Text = "Подключиться";
                    isConnected = false;
                }
                catch (Exception ex)
                {
                    //AddMessage("Ошибка при отключении: " + ex.Message);
                    MessageBox.Show("[C1002] " + "Ошибка при подключении: " + ex.Message, "Ошибка", MessageBoxButtons.OK);
                }
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort.ReadLine();
                this.Invoke(new Action(() =>
                {
                    // Обновляем данные на графиках
                    UpdateCharts(data);

                    // Отображение данных в TextBox
                    txtData.AppendText(data + Environment.NewLine);
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("[R1001] " + "Ошибка при получении данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK);
            }
        }

        private bool isErrorHandled = false;

        private void AddMessage(string message)
        {
            // txtMessages.AppendText(message + Environment.NewLine);

            try
            {
                txtMessages.AppendText(message + Environment.NewLine);

                isErrorHandled = false;
            }
            catch (Exception ex)
            {
                if (!isErrorHandled)
                {
                    isErrorHandled = true;
                    MessageBox.Show("[C2001] " + ex.Message, "Ошибка", MessageBoxButtons.OK);
                    
                }
                
            }
        }


        // Метод для сохранения графика в файл
        // Метод для сохранения графиков
        private void btnSaveChart_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Files|*.png";
            saveFileDialog.Title = "Сохранить графики как изображение";
            saveFileDialog.FileName = "chart.png";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;

                // Сохранение первого графика (высота)
                Bitmap bmp1 = new Bitmap(cartesianChart1.Width, cartesianChart1.Height);
                cartesianChart1.DrawToBitmap(bmp1, new Rectangle(0, 0, cartesianChart1.Width, cartesianChart1.Height));
                bmp1.Save(fileName.Replace(".png", "_altitude.png"), ImageFormat.Png);

                // Сохранение второго графика (ускорение)
                Bitmap bmp2 = new Bitmap(cartesianChart2.Width, cartesianChart2.Height);
                cartesianChart2.DrawToBitmap(bmp2, new Rectangle(0, 0, cartesianChart2.Width, cartesianChart2.Height));
                bmp2.Save(fileName.Replace(".png", "_acceleration.png"), ImageFormat.Png);

                AddMessage("Графики успешно сохранены!");
            }
        }

        // Метод для сохранения данных в файл
        // Метод для сохранения данных в файл
        private void btnSaveData_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files|*.txt";
            saveFileDialog.Title = "Сохранить данные";
            saveFileDialog.FileName = "data.txt";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                File.WriteAllText(fileName, txtData.Text);  // txtData — это TextBox, где отображаются данные

                AddMessage("Данные успешно сохранены!");
            }
        }

        

    }
}
