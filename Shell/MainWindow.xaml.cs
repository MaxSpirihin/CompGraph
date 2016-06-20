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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Shell
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WriteableBitmap bmp;

        public MainWindow()
        {
            InitializeComponent();
            WriteableBitmap writeableBmp = BitmapFactory.New(512, 512);
            field.Source = writeableBmp;
            Clear();
            // SetPixel(100, 100,Colors.White);
        }


        public void SetPixel(int x, int y, Color color)
        {
            using (bmp.GetBitmapContext())
            {
                bmp.SetPixel(10, 13, color);
            }
        }


        public void Clear()
        {
            using (bmp.GetBitmapContext())
            {
                bmp.Clear(Colors.Black);
            }
        }
    }
}
