using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
// using System.Windows;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.WinForms;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.IO.Compression;
using System.Net;
using System.Diagnostics;
using System.CodeDom;
using System.Threading;
using System.Threading.Tasks;
using Tulpep.NotificationWindow;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Windows.Input;



namespace TeleRC
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort;
        private bool isConnected = false;

        private string logFilePath;


        private int[] latestLidarDistances = new int[19];

        // Отдельные серии для каждой оси ускорения и высоты
        private ChartValues<ObservablePoint> accelXValues;
        private ChartValues<ObservablePoint> accelYValues;
        private ChartValues<ObservablePoint> accelZValues;
        private ChartValues<ObservablePoint> altitudeValues;

        private double timeElapsed = 0.0; // Переменная для отслеживания времени в секундах

        public Form1()
        {
            InitializeComponent();
            LoadAvailablePorts();
            InitializeCharts();
            InitializeLogFile();
            this.comboBoxBaudRate.SelectedIndex = 0;
          //  txtSendData1.KeyDown += txtSendData_KeyDown;
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;


            portRefreshTimer = new System.Windows.Forms.Timer();
            portRefreshTimer.Interval = 3000; // обновление каждые 3 секунды
            portRefreshTimer.Tick += PortRefreshTimer_Tick;
            portRefreshTimer.Start();


        }

        private void PortRefreshTimer_Tick(object sender, EventArgs e)
        {
            var currentPorts = SerialPort.GetPortNames();

            if (!currentPorts.SequenceEqual(previousPorts))
            {

                comboBoxPort.Items.Clear();
                comboBoxPort.Items.AddRange(currentPorts);

                if (currentPorts.Length > 0)
                {
                    comboBoxPort.SelectedIndex = 0;
                }
                else
                {
                    // Можно, например, очистить выбранный порт
                    comboBoxPort.Text = "";
                }

                previousPorts = currentPorts;
            }
        }


        private void BtnSendData_Click(object sender, EventArgs e)
        {
            if (isConnected && serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    string dataToSend = txtSendData.Text;
                    serialPort.WriteLine(dataToSend);
                    ShowToastNotification("Успешно", "Данные отправлены");
                }
                catch (Exception ex)
                {
                    ShowToastNotification("Ошибка", "Ошибка при отправке данных: " + ex.Message);
                }
            }
            else
            {
                showToast("Ошибка", "Порт не подключен");
            }
        }

        private void ShowToastNotification(string title, string message)
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show();
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

        private void InitializeCharts()
        {
            altitudeValues = new ChartValues<ObservablePoint>();
            accelXValues = new ChartValues<ObservablePoint>();
            accelYValues = new ChartValues<ObservablePoint>();
            accelZValues = new ChartValues<ObservablePoint>();

            // График высоты
            cartesianChart1.Series = new SeriesCollection
    {
        new LineSeries
        {
            Title = "Высота",
            Values = altitudeValues,
            StrokeThickness = 2,
            PointGeometrySize = 5
        }
    };

            // График ускорений
            cartesianChart2.Series = new SeriesCollection
    {
        new LineSeries
        {
            Title = "Ускорение X",
            Values = accelXValues,
            StrokeThickness = 2,
            PointGeometrySize = 5
        },
        new LineSeries
        {
            Title = "Ускорение Y",
            Values = accelYValues,
            StrokeThickness = 2,
            PointGeometrySize = 5
        },
        new LineSeries
        {
            Title = "Ускорение Z",
            Values = accelZValues,
            StrokeThickness = 2,
            PointGeometrySize = 5
        }
    };

            // Оси графика высоты
            cartesianChart1.AxisX.Add(new Axis
            {
                Title = "Время (с)",
                LabelFormatter = value => value.ToString("0") // Just display seconds
            });

            cartesianChart1.AxisY.Add(new Axis
            {
                Title = "Высота (м)",
                LabelFormatter = value => value.ToString("0.##")
            });

            // Оси графика ускорений
            cartesianChart2.AxisX.Add(new Axis
            {
                Title = "Время (с)",
                LabelFormatter = value => value.ToString("0") // Just display seconds
            });

            cartesianChart2.AxisY.Add(new Axis
            {
                Title = "Ускорение (м/с²)",
                LabelFormatter = value => value.ToString("0.##")
            });
        }


        private string[] dataFieldsOrder;

        

        private void UpdateCharts(string data)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateCharts(data)));
                return;
            }

            string[] values = data.Split(';');

            if (values.Length != 12) return; // Проверка на достаточное количество значений

            // Извлекаем время из данных
            if (double.TryParse(values[1], out double time)) // Время на индекс 1
            {
                timeElapsed = time / 1000.0; // Конвертируем миллисекунды в секунды
            }

            // Извлекаем высоту и ускорения
            if (double.TryParse(values[2], out double altitude)) // Высота на индекс 2
            {
                altitudeValues.Add(new ObservablePoint(timeElapsed, altitude / 100.0)); // Конвертируем в метры
            }

            if (double.TryParse(values[3], out double accelX)) // aX на индекс 3
            {
                accelXValues.Add(new ObservablePoint(timeElapsed, accelX * 9.81 / 100.0)); // Конвертируем в m/s²
            }

            if (double.TryParse(values[4], out double accelY)) // aY на индекс 4
            {
                accelYValues.Add(new ObservablePoint(timeElapsed, accelY * 9.81 / 100.0)); // Конвертируем в m/s²
            }

            if (double.TryParse(values[5], out double accelZ)) // aZ на индекс 5
            {
                accelZValues.Add(new ObservablePoint(timeElapsed, accelZ * 9.81 / 100.0)); // Конвертируем в m/s²
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
                    ShowToastNotification("Успешно", "Подключено к порту " + selectedPort + " со скоростью " + baudRate);
                    btnConnectDisconnect.Text = "Отключиться";
                    isConnected = true;
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("[C1001] " + "Ошибка при подключении: " + ex.Message, "Ошибка", MessageBoxButtons.OK);
                    ShowToastNotification("Ошибка", "Ошибка при подключении: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    serialPort.Close();
                    ShowToastNotification("Внимание", "Отключено");
                    btnConnectDisconnect.Text = "Подключиться";
                    isConnected = false;
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("[C1002] " + "Ошибка при отключении: " + ex.Message, "Ошибка", MessageBoxButtons.OK);
                    ShowToastNotification("Ошибка", "Ошибка при отключении: " + ex.Message);
                }
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort.ReadLine().Trim();

                this.Invoke(new Action(() =>
                {
                    if (IsLidarData(data))
                    {
                        VisualizeLidarData(data);
                    }
                    else
                    {
                        UpdateCharts(data);
                    }

                    // Отображение данных в TextBox
                    txtData.AppendText(data + Environment.NewLine);
                    SaveTelemetryToFile(data);
                }));
            }
            catch (Exception ex)
            {
                showToast("Ошибка", "Ошибка при получении данных: " + ex.Message);
            }
        }



        private bool IsLidarData(string data)
        {
            var parts = data.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 19)
            {
                return parts.All(p => int.TryParse(p, out _));
            }

            return false;
        }


        private void VisualizeLidarData(string data)
        {
            // Очищаем старые данные
            latestLidarDistances = null;

            // Разбираем новые данные
            var distances = data.Split(new[] { ' ', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => int.Parse(s))
                                .ToArray();

            latestLidarDistances = distances;
            pictureBoxLidar.Invalidate(); // Перерисовать
        }



        private void pictureBoxLidar_Paint(object sender, PaintEventArgs e)
        {
            if (latestLidarDistances == null || latestLidarDistances.Length != 19) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.Black);

            int centerX = pictureBoxLidar.Width / 2;
            int centerY = pictureBoxLidar.Height / 2;
            int maxRadius = Math.Min(centerX, centerY) - 10;

            Pen pen = new Pen(Color.Lime, 2);

            for (int i = 0; i < latestLidarDistances.Length; i++)
            {
                double angleDeg = i * 10;
                double angleRad = angleDeg * Math.PI / 180;

                // Нормализуем расстояние в диапазон [0, maxRadius]
                int dist = Math.Min(latestLidarDistances[i], 200); // Ограничим макс. радиус
                double scale = (double)dist / 200;
                int radius = (int)(scale * maxRadius);

                int x = centerX + (int)(radius * Math.Cos(angleRad));
                int y = centerY - (int)(radius * Math.Sin(angleRad)); // Минус — чтобы верх был 90°

                g.DrawLine(pen, centerX, centerY, x, y);
                g.FillEllipse(Brushes.Red, x - 3, y - 3, 6, 6);
            }

            pen.Dispose();
        }



        // Метод для сохранения графика в файл
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

                ShowToastNotification("Успешно", "Графики сохранены!");
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

                ShowToastNotification("Успешно", "Данные сохранены!");

            }
        }

        private void comboBoxPort_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnClearCharts_Click(object sender, EventArgs e)
        {
            ClearChartData();
        }

        private void ClearChartData()
        {

            altitudeValues.Clear();
            accelXValues.Clear();
            accelYValues.Clear();
            accelZValues.Clear();

            ShowToastNotification("Внимание", "Данные графиков очищены.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        public void showToast(string type, string message)
        {
            
        }

        private void comboBoxPort_OnSelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtSendData__TextChanged(object sender, EventArgs e)
        {

        }

        private void txtData_TextChanged(object sender, EventArgs e)
        {

        }

        private void SaveTelemetryToFile(string data)
        {
            try
            {
                if (string.IsNullOrEmpty(logFilePath))
                {
                    ShowToastNotification("Ошибка", "Лог-файл не инициализирован.");
                    return;
                }

                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(data);
                }
            }
            catch (Exception ex)
            {
                ShowToastNotification("Ошибка", $"Ошибка при сохранении данных в файл: {ex.Message}");
            }
        }



        private void InitializeLogFile()
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string chartsPath = Path.Combine(appDataPath, "Charts", "Logs");

                if (!Directory.Exists(chartsPath))
                {
                    Directory.CreateDirectory(chartsPath);
                }

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                logFilePath = Path.Combine(chartsPath, $"telemetry_{timestamp}.log");

                
            }
            catch (Exception ex)
            {
                ShowToastNotification("Ошибка", $"[F1002] Ошибка при инициализации лог-файла: {ex.Message}");

            }
        }

        private void comboBoxBaudRate_OnSelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtSendData_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                showToast("121", "1231");
                if (isConnected && serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    string dataToSend = txtSendData.Text;
                    serialPort.WriteLine(dataToSend);
                    ShowToastNotification("Успешно", "Данные отправлены");
                }
                catch (Exception ex)
                {
                    ShowToastNotification("Ошибка", "Ошибка при отправке данных: " + ex.Message);
                }
            }
            else
            {
                showToast("Ошибка", "Порт не подключен");
            }
            }
        }

        private void Form1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)

        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                // Проверка: есть ли текст
                if (!string.IsNullOrWhiteSpace(txtSendData.Texts))
                {
                    BtnSendData_Click(sender, EventArgs.Empty);
                    txtSendData.Texts = ""; 
                }
            }
        }



    }
}
