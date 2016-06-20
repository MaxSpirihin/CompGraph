using Graph;
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
using ShellExt;

namespace Lab1Demo
{

    enum Task
    {
        LINE,
        POLYGON
    }


    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IPixelPressListener
    {
        private readonly List<List<int>> TEST_DATA = new List<List<int>>()
        {
            //1-треугольник
            new List<int>
            {
                100,100,250,400,400,100
            },
            //2 - невыпуклый
            new List<int> 
            {
                100,100,250,400,400,100, 250,200
            },
            //3 - сложный
            new List<int> 
            {
                100,100,400,400,100,400, 400,100
            },
            //4 - звзда
            new List<int> 
            {
                150,100,300,300,100,300, 250,100,200,400
            },
            //5 - EO vs NZW
            new List<int> 
            {
                50,200,450,200,450,100,100,250,400,250,50,100
            }
        };

        Task task;
        LineCap cap;

        int x1, y1, x2, y2;
        bool first = true;
        bool isEntering = false;

        ShellExt.Polygon pol = new ShellExt.Polygon();

        public MainWindow()
        {
            InitializeComponent();
            Shell.Instance.AddListener(this);
        }


        public void onPressPixel(int x, int y)
        {
            switch (task)
            {
                case Task.LINE:
                    OnPressPixelLine(x, y);
                    break;
                case Task.POLYGON:
                    OnPressPixelPolygon(x, y);
                    break;
            }
        }


        void OnPressPixelLine(int x, int y)
        {
            if (first)
            {
                x1 = x;
                y1 = y;
                Shell.Instance.SetPixel(x, y);
            }
            else
            {
                x2 = x;
                y2 = y;

                bool isComplicated = CBComplicatedLine.IsChecked.Value;
                bool isDashed = CBDashedLine.IsChecked.Value;
                if (isDashed)
                {
                    if (isComplicated)
                        Shell.Instance.DrawLineDashed(x1, y1, x2, y2, Convert.ToInt32(SLWidth.Value));
                    else
                        Shell.Instance.DrawLineDashed(x1, y1, x2, y2);
                }
                else
                    if (isComplicated)
                        Shell.Instance.DrawLine(x1, y1, x2, y2, Convert.ToInt32(SLWidth.Value),
                            cap);
                    else
                        Shell.Instance.DrawLine(x1, y1, x2, y2);
            }
            first = !first;
        }

        void OnPressPixelPolygon(int x, int y)
        {
            if (isEntering)
            {
                Shell.Instance.SetPixel(x, y);
                pol.AddPoint(x, y);
                TBPointsCount.Text = "PointsCount - " + pol.Length.ToString();

                if (pol.Length > 2)
                {
                    BTNStroke.IsEnabled = true;
                    BTNFillEO.IsEnabled = true;
                    BTNFillNZW.IsEnabled = true;
                    TBType.Text = "Type: " + (!pol.IsSimple ? "complicated" : ("simple, " +
                   (pol.IsConvex ? "convex" : "nonconvex")));
                }
            }
        }

        private void CMBCap_Selected(object sender, RoutedEventArgs e)
        {
            int index = CMBCap.SelectedIndex;
            cap = index == 0 ? LineCap.FLAP : index == 1 ? LineCap.ROUND : LineCap.SQUARE;
        }

        private void LineCBChange(object sender, RoutedEventArgs e)
        {
            if (CBComplicatedLine.IsChecked.Value)
            {
                SLWidth.IsEnabled = true;

                if (CBDashedLine.IsChecked.Value)
                    CMBCap.IsEnabled = false;
                else
                    CMBCap.IsEnabled = true;

            }
            else
            {
                SLWidth.IsEnabled = false;
                CMBCap.IsEnabled = false;
            }



        }


        private void CMBTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CMBTask.SelectedIndex == 0)
                {
                    task = Task.LINE;
                    LineTask.Visibility = System.Windows.Visibility.Visible;
                    PolygonTask.Visibility = System.Windows.Visibility.Collapsed;
                    first = true;
                }
                else
                {
                    task = Task.POLYGON;
                    LineTask.Visibility = System.Windows.Visibility.Collapsed;
                    PolygonTask.Visibility = System.Windows.Visibility.Visible;
                    BTNStroke.IsEnabled = false;
                    BTNFillEO.IsEnabled = false;
                    BTNFillNZW.IsEnabled = false;
                    isEntering = false;
                }
                Shell.Instance.Clear();
            }
            catch (Exception)
            {

            }
        }

        private void SLWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TBWidth.Text = SLWidth.Value.ToString();
        }

        private void BTNStartEnter_Click(object sender, RoutedEventArgs e)
        {
            Shell.Instance.Clear();
            pol = new ShellExt.Polygon();
            BTNStroke.IsEnabled = false;
            BTNFillEO.IsEnabled = false;
            BTNFillNZW.IsEnabled = false;
            isEntering = true;
            TBPointsCount.Text = "PointsCount - 0";
            TBType.Text = "";
        }

        private void BTNStroke_Click(object sender, RoutedEventArgs e)
        {
            Shell.Instance.StrokePolygon(pol, Colors.Red);
            isEntering = false;
        }

        private void BTNFill_Click(object sender, RoutedEventArgs e)
        {
            Shell.Instance.FillPolygon(pol);
            isEntering = false;
        }

        private void BTNFillNZW_Click(object sender, RoutedEventArgs e)
        {
            Shell.Instance.FillPolygon(pol, true);
            isEntering = false;
        }

        private void BTNTestEnter_Click(object sender, RoutedEventArgs e)
        {
            BTNStartEnter_Click(null, null);
            List<int> data = TEST_DATA[CMBTestPol.SelectedIndex];

            for (int i = 0; i < data.Count() - 1; i += 2)
            {
                onPressPixel(data[i], data[i + 1]);
            }

            isEntering = false;
        }



        public void onMove(int x, int y)
        {
        }

        public void onOut(int x, int y)
        {
        }
    }
}
