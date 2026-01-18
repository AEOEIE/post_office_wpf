using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace kurs4v28
{
    /// <summary>
    /// Логика взаимодействия для ZastavochkaWindow.xaml
    /// </summary>
    public partial class ZastavochkaWindow : Window
    {
        public ZastavochkaWindow()
        {
            InitializeComponent();
            LoadImageWithCode();
            Loaded += ZastavochkaWindow_Loaded;
        }

        private void ZastavochkaWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var animation = this.FindResource("SceneAnimation") as Storyboard;
            animation.Begin();

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3); // Общее время
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                ShowMainWindow();
            };
            timer.Start();
        }
        private void LoadImageWithCode()
        {
            try
            {
                string debugPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string imagePath = System.IO.Path.Combine(debugPath, "bad_apple.jpg");

                if (System.IO.File.Exists(imagePath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    var imageControl = (Image)FindName("PostImage");
                    if (imageControl != null)
                    {
                        imageControl.Source = bitmap;
                    }
                }
                else
                {
                    MessageBox.Show($"Файл не найден: {imagePath}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }
        private void ShowMainWindow()
        {
            
            MainWindow mainWindow = new MainWindow()
            {
                WindowStartupLocation = WindowStartupLocation,
                Left = Left,
                Top = Top
            };
            mainWindow.Show();
            this.Close();
        }
    }
}
