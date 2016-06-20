using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Graph
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    partial class MainWindow : Window, IPixelPressListener
    {
        private const int ZOOM_SIZE = 8;
        private const int SIZE = 512;
        private WriteableBitmap bmp;
        private List<IPixelPressListener> listeners;
        bool IsZoom = false;
        bool mouseIn = false;

        public MainWindow()
        {
            InitializeComponent();
            listeners = new List<IPixelPressListener>();
            bmp = BitmapFactory.New(SIZE, SIZE);
            field.Source = bmp;
            Clear();
            AddListener(this);
            
        }


        public void SetPixel(int x, int y, Color color)
        {
            x += (int)bmp.PixelWidth / 2;
            y += (int)bmp.PixelWidth / 2;

            if (IsZoom)
            {
                if (!(x < 0 || x >= bmp.PixelWidth / ZOOM_SIZE || y < 0 || y >= bmp.PixelHeight / ZOOM_SIZE))
                    bmp.FillRectangle(x * ZOOM_SIZE, y * ZOOM_SIZE, x * ZOOM_SIZE + ZOOM_SIZE, y * ZOOM_SIZE + ZOOM_SIZE, color);
                return;
            }

            if (!(x < 0 || x >= bmp.PixelWidth || y < 0 || y >= bmp.PixelHeight))
                bmp.SetPixel(x, y, color);
        }


        public void Clear()
        {
            bmp.Clear(Shell.ClEAR_COLOR);
        }

        public Color GetPixel(int x, int y)
        {
            if (IsZoom)
            {
                x = x / ZOOM_SIZE;
                y = y / ZOOM_SIZE;
            }

            return bmp.GetPixel(x, y);
        }


        public void AddListener(IPixelPressListener listener)
        {
            if (!listeners.Contains(listener))
                listeners.Add(listener);
        }

        private void PressField(object sender, MouseButtonEventArgs e)
        {
            //координты мыши
            mouseIn = true;
            Vector mouse = GetMousePosition(e);

            foreach (IPixelPressListener listener in listeners)
                listener.onPressPixel(Convert.ToInt32(Math.Truncate(mouse.X)), Convert.ToInt32(Math.Truncate(mouse.Y)));
        }


        private void OutField(object sender, MouseButtonEventArgs e)
        {
            //координты мыши
            mouseIn = false;
            Vector mouse = GetMousePosition(e);

            foreach (IPixelPressListener listener in listeners)
                listener.onOut(Convert.ToInt32(Math.Truncate(mouse.X)), Convert.ToInt32(Math.Truncate(mouse.Y)));
        }

        private void MoveField(object sender, MouseEventArgs e)
        {
            if (!mouseIn) return;
            Vector mouse = GetMousePosition(e);

            foreach (IPixelPressListener listener in listeners)
                listener.onMove(Convert.ToInt32(Math.Truncate(mouse.X)), Convert.ToInt32(Math.Truncate(mouse.Y)));

        }


        private Vector GetMousePosition(MouseEventArgs e)
        {
            Vector mouse = new Vector(e.GetPosition(field).X, e.GetPosition(field).Y);

            if (IsZoom)
            {
                mouse.X = mouse.X / ZOOM_SIZE;
                mouse.Y = mouse.Y / ZOOM_SIZE;
            }

            return mouse;
        }


        private void Save(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "LogiCC File (*.bmp)|*.bmp";
            if (saveFileDialog.ShowDialog() == true)
            {
                CreateThumbnail(saveFileDialog.FileName,bmp.Clone());
            }
        }

        void CreateThumbnail(string filename, BitmapSource image5)
        {
            if (filename != string.Empty)
            {
                using (FileStream stream5 = new FileStream(filename, FileMode.Create))
                {
                    PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                    encoder5.Frames.Add(BitmapFrame.Create(image5));
                    encoder5.Save(stream5);
                    stream5.Close();
                }
            }
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void Zoom(object sender, RoutedEventArgs e)
        {
            Clear();
            IsZoom = !IsZoom;
            ZoomBtn.Header = IsZoom ? "Off Zoom" : "On Zoom";
        }

        private void About(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Created By Maxim Spirikhin (k5-171)", "About GFrame");
        }

        void IPixelPressListener.onPressPixel(int x, int y)
        {
          //  SetPixel(x, y, Colors.Wheat);
        }


        public void onMove(int x, int y)
        {
            //SetPixel(x, y, Colors.Coral);
        }

        public void onOut(int x, int y)
        {
            //SetPixel(x, y, Colors.Chocolate);
        }

        
    }
}
