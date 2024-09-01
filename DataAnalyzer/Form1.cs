using System;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.WinForms;
using LiveCharts.Defaults;

namespace DataAnalyzer
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort;

        private LineSeries altitudeSeries;
        private LineSeries accelXSeries;
        private LineSeries accelYSeries;
        private LineSeries accelZSeries;
        private LineSeries accelTotalSeries;

        public Form1()
        {
            InitializeComponent();

            // Настраиваем графики
            altitudeSeries = new LineSeries
            {
                Values = new ChartValues<ObservablePoint>(),
                Stroke = System.Windows.Media.Brushes.Blue,
                Fill = System.Windows.Media.Brushes.Transparent,
                Title = "Линия зависимости"
            };

            accelXSeries = new LineSeries
            {
                Values = new ChartValues<ObservablePoint>(),
                Stroke = System.Windows.Media.Brushes.Red,
                Fill = System.Windows.Media.Brushes.Transparent,
                Title = "X"
            };

            accelYSeries = new LineSeries
            {
                Values = new ChartValues<ObservablePoint>(),
                Stroke = System.Windows.Media.Brushes.Green,
                Fill = System.Windows.Media.Brushes.Transparent,
                Title = "Y"
            };

            accelZSeries = new LineSeries
            {
                Values = new ChartValues<ObservablePoint>(),
                Stroke = System.Windows.Media.Brushes.Orange,
                Fill = System.Windows.Media.Brushes.Transparent,
                Title = "Z"
            };

            accelTotalSeries = new LineSeries
            {
                Values = new ChartValues<ObservablePoint>(),
                Stroke = System.Windows.Media.Brushes.Purple,
                Fill = System.Windows.Media.Brushes.Transparent,
                Title = "Абсолютное ускорение"
            };

            cartesianChart1.Series.Add(altitudeSeries);
            cartesianChart2.Series.Add(accelXSeries);
            cartesianChart2.Series.Add(accelYSeries);
            cartesianChart2.Series.Add(accelZSeries);
            cartesianChart2.Series.Add(accelTotalSeries);

            // Настроим оси и заголовки
            cartesianChart1.AxisX.Add(new Axis { Title = "Time (s)" });
            cartesianChart1.AxisY.Add(new Axis { Title = "Altitude (m)" });
            cartesianChart1.LegendLocation = LegendLocation.Right;
            // cartesianChart1.Title = "Altitude vs Time"; // Удалено, так как свойство Title может отсутствовать

            cartesianChart2.AxisX.Add(new Axis { Title = "Time (s)" });
            cartesianChart2.AxisY.Add(new Axis { Title = "Acceleration (m/s²)" });
            cartesianChart2.LegendLocation = LegendLocation.Right;
            // cartesianChart2.Title = "Acceleration vs Time"; // Удалено, так как свойство Title может отсутствовать
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Поиск доступных COM-портов
            comboBoxPorts.Items.AddRange(SerialPort.GetPortNames());
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (serialPort == null)
            {
                serialPort = new SerialPort(comboBoxPorts.SelectedItem.ToString(), 9600);
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();
                btnConnect.Text = "Отключить";
            }
            else
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                    serialPort.Dispose();
                    serialPort = null;
                }
                btnConnect.Text = "Подключить";
            }
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort.ReadLine();
                this.Invoke(new Action(() =>
                {
                    txtData.AppendText(data + Environment.NewLine);

                    // Парсинг данных
                    var parts = data.Split(';');
                    if (parts.Length == 10)
                    {
                        // Чтение данных
                        float time = float.Parse(parts[1]) / 1000; // время в секундах
                        float altitude = float.Parse(parts[2]) / 100; // высота в метрах
                        float accelX = float.Parse(parts[3]) * 9.81f / 100; // акселерометр X
                        float accelY = float.Parse(parts[4]) * 9.81f / 100; // акселерометр Y
                        float accelZ = float.Parse(parts[5]) * 9.81f / 100; // акселерометр Z

                        // Суммарное ускорение
                        float accelMagnitude = (float)Math.Sqrt(accelX * accelX + accelY * accelY + accelZ * accelZ);

                        // Добавление точек на графики
                        altitudeSeries.Values.Add(new ObservablePoint(time, altitude));
                        accelXSeries.Values.Add(new ObservablePoint(time, accelX));
                        accelYSeries.Values.Add(new ObservablePoint(time, accelY));
                        accelZSeries.Values.Add(new ObservablePoint(time, accelZ));
                        accelTotalSeries.Values.Add(new ObservablePoint(time, accelMagnitude));

                        // Ограничиваем количество точек на графиках
                        if (altitudeSeries.Values.Count > 100)
                        {
                            altitudeSeries.Values.RemoveAt(0);
                            accelXSeries.Values.RemoveAt(0);
                            accelYSeries.Values.RemoveAt(0);
                            accelZSeries.Values.RemoveAt(0);
                            accelTotalSeries.Values.RemoveAt(0);
                        }
                    }
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при чтении данных: " + ex.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Остановим поток и закроем порт перед выходом
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                serialPort.Dispose();
            }
        }

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Сохранить данные"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, txtData.Text);
                MessageBox.Show("Данные сохранены!");
            }
        }

        private void btnSaveCharts_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG Image|*.png";
                saveFileDialog.Title = "Сохранить графики";

                // Выбор имени файла для первого графика
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Сохранение первого графика (Altitude vs Time)
                        string fileName1 = Path.GetFileNameWithoutExtension(saveFileDialog.FileName) + "_Altitude.png";
                        string filePath1 = Path.Combine(Path.GetDirectoryName(saveFileDialog.FileName), fileName1);
                        using (Bitmap bitmap1 = new Bitmap(cartesianChart1.Width, cartesianChart1.Height))
                        {
                            cartesianChart1.DrawToBitmap(bitmap1, new Rectangle(0, 0, bitmap1.Width, bitmap1.Height));
                            bitmap1.Save(filePath1);
                        }

                        // Сохранение второго графика (Acceleration vs Time)
                        string fileName2 = Path.GetFileNameWithoutExtension(saveFileDialog.FileName) + "_Acceleration.png";
                        string filePath2 = Path.Combine(Path.GetDirectoryName(saveFileDialog.FileName), fileName2);
                        using (Bitmap bitmap2 = new Bitmap(cartesianChart2.Width, cartesianChart2.Height))
                        {
                            cartesianChart2.DrawToBitmap(bitmap2, new Rectangle(0, 0, bitmap2.Width, bitmap2.Height));
                            bitmap2.Save(filePath2);
                        }

                        MessageBox.Show("Графики сохранены!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при сохранении графиков: " + ex.Message);
                    }
                }
            }
        }
    }
}