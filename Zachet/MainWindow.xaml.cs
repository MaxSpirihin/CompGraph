using _3D;
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
using System.Windows.Threading;
using ShellExt;

namespace Lab3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Parallelepiped p;
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            p = new Parallelepiped()
            {
                Center = new Vector3(0, 0, -50),
                Size = new Vector3(100, 100, 100),
                Turn = new Vector3(270, 90, 0)
            };
            Draw();


        }

        private void TurnY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
             try
            {
            if (timer.IsEnabled) return;
            p.Turn.y = TurnY.Value;
            Draw();
                 }
            catch (Exception) { }
        }



        void Draw()
        {
            Shell.Instance.Clear();
        //    if (IsPerspective.IsChecked.Value)
         //       p.DrawPerspective(Shell.Instance, k, DelFringes.IsChecked.Value);
        //    else
         //       p.DrawParallel(Shell.Instance, DelFringes.IsChecked.Value);
            


            Vector3 p1 = p.Points[0];
            Vector3 p2 = p.Points[1];
            Vector3 p3 = p.Points[2];
            Vector3 p4 = p.Points[3];

            Vector3 n = (p1 - p2) * (p2 - p3);
            n = n / n.Length;

            Vector3 p12 = p1 + (p2 - p1) * p12W.Value + n * p12N.Value,
                p13 = p1 + (p3 - p1) * p13W.Value + n * p13N.Value,
                p24 = p2 + (p4 - p2) * p24W.Value + n * p24N.Value,
                p34 = p3 + (p4 - p3) * p34W.Value + n * p34N.Value;

            double k =150;

            if (MainPoints.IsChecked.Value)
            {
                Shell.Instance.DrawCircle((int)p1.PerspectiveProection(k).x, (int)p1.PerspectiveProection(k).y, 5, Colors.Red);
                Shell.Instance.DrawCircle((int)p2.PerspectiveProection(k).x, (int)p2.PerspectiveProection(k).y, 5, Colors.Red);
                Shell.Instance.DrawCircle((int)p3.PerspectiveProection(k).x, (int)p3.PerspectiveProection(k).y, 5, Colors.Red);
                Shell.Instance.DrawCircle((int)p4.PerspectiveProection(k).x, (int)p4.PerspectiveProection(k).y, 5, Colors.Red);
            }
            if (EtxtraPoints.IsChecked.Value)
            {
                Shell.Instance.DrawCircle((int)p12.PerspectiveProection(k).x, (int)p12.PerspectiveProection(k).y, 5, Colors.Blue);
                Shell.Instance.DrawCircle((int)p13.PerspectiveProection(k).x, (int)p13.PerspectiveProection(k).y, 5, Colors.Blue);
                Shell.Instance.DrawCircle((int)p24.PerspectiveProection(k).x, (int)p24.PerspectiveProection(k).y, 5, Colors.Blue);
                Shell.Instance.DrawCircle((int)p34.PerspectiveProection(k).x, (int)p34.PerspectiveProection(k).y, 5, Colors.Blue);

            }




            _3D.Surface.Grid grid = Surface.CunsAndBezier(p1, p2, p3, p4,
                p12,p13,p24,p34,(int)Grid.Value);
            if (!AllLines.IsChecked.Value)
                grid.DrawWithZBuf(Shell.Instance, 150);
            else
                grid.Draw(Shell.Instance, 150);
        }



        private void CenterX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (timer.IsEnabled) return;
                p.Center.x = CenterX.Value;
                Draw();
            }
            catch (Exception) { }
        }

        private void CenterY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (timer.IsEnabled) return;
                p.Center.y = CenterY.Value;
                Draw();
            }
            catch (Exception) { }
        }


        private void IsPerspective_Checked(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled) return;
            Draw();
        }

        private void SizeX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (timer.IsEnabled) return;
                p.Size.x = SizeX.Value;
                Draw();
            }
            catch (Exception) { }
        }

        private void SizeY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (timer.IsEnabled) return;
                p.Size.y = SizeY.Value;
                Draw();
            }
            catch (Exception) { }
        }





        private void AllLines_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                Draw();
            }
            catch (Exception) { }
        }

    }
}
