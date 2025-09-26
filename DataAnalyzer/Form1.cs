using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.Diagnostics;
using System.Linq;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Runtime.InteropServices;
using System.Text;




namespace TeleRC
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort;
        private bool isConnected = false;

        private string logFilePath;

        private bool isClosing = false;

        private int seaLevelPressure = 101325;

        private int[] latestLidarDistances = new int[19];

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
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;


            portRefreshTimer = new System.Windows.Forms.Timer();
            portRefreshTimer.Interval = 3000;
            portRefreshTimer.Tick += PortRefreshTimer_Tick;
            portRefreshTimer.Start();
        }

        private double calculateAltitude(double pressure, int seaLevelPressure)
        {
            return (44330.0 * (1.0 - Math.Pow(pressure / seaLevelPressure, 0.1903)) * 100);
        }



        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct RadioData
        {
            public ushort start_of_packet;
            public ushort team_id;
            public uint time;
            public short temperature;
            public int pressure;
            public short accelerometer_x;
            public short accelerometer_y;
            public short accelerometer_z;
            public short gyroscope_x;
            public short gyroscope_y;
            public short gyroscope_z;
            public byte checksum;
            public byte answer;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public ushort[] distanses;
        }

        private const int RADIO_DATA_SIZE = 2 + 2 + 4 + 2 + 4 + 2 * 6 + 1 + 1 + 17 * 2;

        private static RadioData ByteArrayToRadioData(byte[] bytes)
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return Marshal.PtrToStructure<RadioData>(handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }
        }

        private static string FormatRadioData(RadioData d)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{d.team_id};{d.time};{d.temperature};{d.pressure};");
            sb.Append($"{d.accelerometer_x};{d.accelerometer_y};{d.accelerometer_z};");
            sb.Append($"{d.gyroscope_x};{d.gyroscope_y};{d.gyroscope_z}");
            foreach (ushort dist in d.distanses)
                sb.Append($";{dist}");
            return sb.ToString();
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
                    string getstr = txtSendData.Text;
                    string dataToSend = $"{getstr}\n";
                    serialPort.Write(dataToSend);

                }
                catch (Exception ex)
                {
                    ShowToastNotification("Ошибка", "Ошибка при отправке данных: " + ex.Message);
                }
            }
            else
            {

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
                LabelFormatter = value => value.ToString("0")
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
                LabelFormatter = value => value.ToString("0")
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

            if (values.Length < 27) return; // 10 графиков + 17 лидара = минимум 27

            if (double.TryParse(values[1], out double time)) // Время на индекс 1
            {
                timeElapsed = time / 1000.0; // Конвертируем миллисекунды в секунды
            }

            if (double.TryParse(values[3], out double altitude)) // Высота
                altitudeValues.Add(new ObservablePoint(timeElapsed, calculateAltitude(altitude, seaLevelPressure) / 100.0));

            if (double.TryParse(values[4], out double accelX)) // aX
                accelXValues.Add(new ObservablePoint(timeElapsed, accelX * 9.81 / 100.0));

            if (double.TryParse(values[5], out double accelY)) // aY
                accelYValues.Add(new ObservablePoint(timeElapsed, accelY * 9.81 / 100.0));

            if (double.TryParse(values[6], out double accelZ)) // aZ
                accelZValues.Add(new ObservablePoint(timeElapsed, accelZ * 9.81 / 100.0));

            // Ограничение количества точек
            const int MaxPoints = 60;
            if (altitudeValues.Count > MaxPoints) altitudeValues.RemoveAt(0);
            if (accelXValues.Count > MaxPoints) accelXValues.RemoveAt(0);
            if (accelYValues.Count > MaxPoints) accelYValues.RemoveAt(0);
            if (accelZValues.Count > MaxPoints) accelZValues.RemoveAt(0);

            // 👇 Обновим lidar данные
            try
            {
                int[] lidarDistances = values.Skip(10)
                             .Where(s => int.TryParse(s, out _))
                             .Take(17)
                             .Select(s => int.Parse(s))
                             .ToArray();

                if (lidarDistances.Length == 17)
                {
                    latestLidarDistances = lidarDistances;
                    pictureBoxLidar.Invalidate();
                }
            }
            catch
            {

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
                    btnConnectDisconnect.Text = "Отключиться";
                    isConnected = true;
                }
                catch (Exception ex)
                {
                    ShowToastNotification("Ошибка", "Ошибка при подключении: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    isClosing = true;

                    serialPort.Close();

                    isClosing = false;

                    btnConnectDisconnect.Text = "Подключиться";
                    isConnected = false;
                }
                catch (Exception ex)
                {
                    ShowToastNotification("Ошибка", "Ошибка при отключении: " + ex.Message);
                }
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (isClosing || !isConnected || serialPort == null || !serialPort.IsOpen)
                return;
            try
            {
                int bytesToRead = serialPort.BytesToRead;

                if (bytesToRead >= RADIO_DATA_SIZE)
                {

                    byte[] buffer = new byte[RADIO_DATA_SIZE];
                    serialPort.Read(buffer, 0, RADIO_DATA_SIZE);

                    // Проверка начала пакета
                    if (buffer[0] != 0xAA || buffer[1] != 0xAA) return;

                    RadioData packet = ByteArrayToRadioData(buffer);
                    
                    string formatted = FormatRadioData(packet);

                    this.Invoke(new Action(() =>
                    {
                        if (isClosing) return;

                        UpdateCharts(formatted);
                        txtData.AppendText(formatted + Environment.NewLine);
                        SaveTelemetryToFile(formatted);
                    }));
                }
            }
            catch (Exception ex)
            {
                showToast("Ошибка", "Ошибка при получении RadioData: " + ex.Message);
            }

        }




        private bool IsLidarData(string data) // неактивно
        {
            var parts = data.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 19)
            {
                return parts.All(p => int.TryParse(p, out _));
            }

            return false;
        }


        private void VisualizeLidarData(string data) // неактивно
        {
            latestLidarDistances = null;

            var distances = data.Split(new[] { ' ', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => int.Parse(s))
                                .ToArray();

            latestLidarDistances = distances;
            pictureBoxLidar.Invalidate(); // Перерисовать
        }



        private void pictureBoxLidar_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.Black);

            int centerX = pictureBoxLidar.Width / 2;
            int centerY = pictureBoxLidar.Height / 2;
            int maxRadius = Math.Min(centerX, centerY) - 10;

            Pen pen = new Pen(Color.Lime, 2);



            if (latestLidarDistances == null || latestLidarDistances.Length != 17)
            {
                g.DrawString("Нет данных лидара", new Font("Arial", 12), Brushes.White, 10, 10);
                return;
            }

            for (int i = 0; i < 17; i++)
            {
                double angleDeg = i * (180.0 / 16);
                double angleRad = angleDeg * Math.PI / 180;

                int rawDist = latestLidarDistances[i];
                int dist = Math.Min(rawDist, 200);
                double scale = dist / 200.0;
                int radius = (int)(scale * maxRadius);

                int x = centerX + (int)(radius * Math.Cos(angleRad));
                int y = centerY - (int)(radius * Math.Sin(angleRad));

                g.DrawLine(pen, centerX, centerY, x, y);
                g.FillEllipse(Brushes.Red, x - 4, y - 4, 8, 8);
            }

            pen.Dispose();
        }




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

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files|*.txt";
            saveFileDialog.Title = "Сохранить данные";
            saveFileDialog.FileName = "data.txt";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                File.WriteAllText(fileName, txtData.Text);

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


            txtData.Clear();
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

        private void OpenSysFile_Click(object sender, EventArgs e)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string chartsPath = Path.Combine(appDataPath, "Charts");

            // Папка существует?
            if (!Directory.Exists(chartsPath))
            {
                Directory.CreateDirectory(chartsPath);
            }

            // Открываем папку
            Process.Start("explorer.exe", chartsPath);
        }

        


    }
}
