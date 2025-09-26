using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TeleRC;
using Microsoft.Toolkit.Uwp.Notifications;
using YourNamespace;

namespace DataParserApp
{
    public partial class SplashForm : Form
    {
        private string pastebinUrl = "https://pastebin.com/raw/Vp1n8BGp";
        private string[] logoImages;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn  
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        private static readonly string SettingsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Charts");
        private static readonly string LastImageIndexFile = Path.Combine(SettingsFolder, "lastImageIndex.txt");
        private static readonly string UrlsHashFile = Path.Combine(SettingsFolder, "urlsHash.txt");
        private static readonly string ImagesFolder = Path.Combine(SettingsFolder, "Images");

        public SplashForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 60, 60));
            CustomizePictureBox();
            LoadImageUrls();
        }

        private void CustomizePictureBox()
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            SetRoundedCorners(pictureBox1, 60);
        }

        private void SetRoundedCorners(PictureBox pictureBox, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(pictureBox.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(pictureBox.Width - radius, pictureBox.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, pictureBox.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();

            pictureBox.Region = new Region(path);
        }

        private void LoadImageUrls()
        {
            try
            {
                string pastebinContent = new WebClient().DownloadString(pastebinUrl);
                string[] newLogoImages = pastebinContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                string newHash = ComputeHash(pastebinContent);

                
                if (!IsHashUpdated(newHash))
                {
                    LoadImagesFromLocalStorage();
                }
                else
                {

                    SaveHash(newHash);
                    logoImages = newLogoImages;
                    DeleteLocalImages();
                    SaveUrlsLocally();
                    SaveImagesLocally();
                }
            }
            catch
            {
                
                LoadImagesFromLocalStorage();
                ShowToastNotification("Ошибка загрузки данных", "Не удалось загрузить изображения с Pastebin. Используются локальные файлы.");
            }
        }

        private string ComputeHash(string content)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private bool IsHashUpdated(string newHash)
        {
            if (!File.Exists(UrlsHashFile)) return true;

            string savedHash = File.ReadAllText(UrlsHashFile);
            return savedHash != newHash;
        }

        private void SaveHash(string newHash)
        {
            EnsureSettingsDirectoryExists();
            File.WriteAllText(UrlsHashFile, newHash);
        }

        private void SaveUrlsLocally()
        {
            EnsureSettingsDirectoryExists();
            string urlsFile = Path.Combine(SettingsFolder, "urls.txt");
            File.WriteAllLines(urlsFile, logoImages);
        }

        private void DeleteLocalImages()
        {
            if (Directory.Exists(ImagesFolder))
            {
                Directory.Delete(ImagesFolder, true);
            }
        }

        private void SaveImagesLocally()
        {
            EnsureSettingsDirectoryExists();

            ProgressForm progressForm = new ProgressForm(logoImages.Length);
            progressForm.Show();
            Application.DoEvents();

            for (int i = 0; i < logoImages.Length; i++)
            {
                try
                {
                    string imageUrl = logoImages[i];
                    string fileName = Path.Combine(ImagesFolder, $"image_{i}.png");

                    using (WebClient webClient = new WebClient())
                    {
                        webClient.DownloadFile(imageUrl, fileName);
                    }

                    // Обновление прогрессбара
                    progressForm.UpdateProgress(i + 1);
                    Application.DoEvents();
                }
                catch
                {
                    // Лог ошибок но я его не напишу мне лень
                }
            }

            progressForm.Close();
        }

        private void LoadImagesFromLocalStorage()
        {
            if (Directory.Exists(ImagesFolder))
            {
                logoImages = Directory.GetFiles(ImagesFolder, "*.png");
            }
            else
            {
                logoImages = new string[0];
            }

            if (logoImages.Length == 0)
            {
                ShowToastNotification("Ошибка", "Локальные изображения отсутствуют. Проверьте подключение к интернету и попробуйте снова.");
            }
        }

        private void ShowToastNotification(string title, string message)
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show();
        }

        private void SplashForm_Load(object sender, EventArgs e)
        {
            int currentImageIndex = GetLastImageIndex();

            if (logoImages.Length > 0)
            {
                pictureBoxLogo.Image = LoadImageFromFile(logoImages[currentImageIndex]);
            }

            SaveNextImageIndex(currentImageIndex);
            timer.Start();
        }

        private int GetLastImageIndex()
        {
            EnsureSettingsDirectoryExists();

            if (File.Exists(LastImageIndexFile))
            {
                try
                {
                    string content = File.ReadAllText(LastImageIndexFile);
                    int lastIndex = int.Parse(content);
                    return (lastIndex + 1) % logoImages.Length;
                }
                catch
                {
                    return 0;
                }
            }
            return 0;
        }

        private void SaveNextImageIndex(int currentIndex)
        {
            EnsureSettingsDirectoryExists();

            int nextIndex = (currentIndex) % logoImages.Length;
            File.WriteAllText(LastImageIndexFile, nextIndex.ToString());
        }

        private Image LoadImageFromFile(string filePath)
        {
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    return new Bitmap(fs); // создаёт копию, освобождает файл
                }
            }
            catch
            {
                ShowToastNotification("Ошибка загрузки изображения", $"Не удалось загрузить локальное изображение: {filePath}");
                return null;
            }
        }


        private void EnsureSettingsDirectoryExists()
        {
            if (!Directory.Exists(SettingsFolder))
            {
                Directory.CreateDirectory(SettingsFolder);
            }

            if (!Directory.Exists(ImagesFolder))
            {
                Directory.CreateDirectory(ImagesFolder);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            this.Hide();

            Form1 mainForm = new Form1();
            mainForm.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
