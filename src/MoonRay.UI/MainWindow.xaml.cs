using System;
using System.Windows;
using System.Windows.Media.Imaging;
using MoonRay.Scene;
using System.IO;
using MoonRay.SceneInterpreter;

namespace MoonRay.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public BitmapImage Convert(System.Drawing.Bitmap src)
        {
            var ms = new MemoryStream();
            src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        private System.Drawing.Color ToWinColor(Color c)
        {
            Func<double, int> threshold = a => (int)System.Math.Floor(255 * (a <= 1.0 ? a : 1.0));
            return System.Drawing.Color.FromArgb(255, threshold(c.R), threshold(c.G), threshold(c.B));
        }

        private void Render(Scene.Scene scene)
        {
            int width = 600;
            int height = 450;

            Color[,] pixels = new Color[width, height];
            
            IRaytracer raytracer = new Raytracer.Raytracer();
            raytracer.RenderScene(scene, width, height, 4, (x, y, color) =>
            {
                pixels[x, y] = color;                
            });
            
            var bmp = new System.Drawing.Bitmap(width, height);
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    bmp.SetPixel(x, y, ToWinColor(pixels[x, y]));

            Display.Source = Convert(bmp);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var desc = SceneDesc.Text;

            RenderButton.IsEnabled = false;

            try
            {
                Status.Text = "Generating scene ...";
                var interpreter = new Interpreter();
                var scene = interpreter.EvalScene(desc);

                Status.Text = "Rendering ...";
                Render(scene);
                Status.Text = "Ready";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                RenderButton.IsEnabled = true;
            }
        }
    }
}
